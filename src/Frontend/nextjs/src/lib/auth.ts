// ============================================================
// OIDC Authentication Module — Authorization Code + PKCE
// ============================================================

import { UserInfo } from '@/types/api';

const AUTH_SERVER_URL = process.env.NEXT_PUBLIC_AUTH_SERVER_URL || 'https://localhost:7133';
const CLIENT_ID = 'bwa-client';
const REDIRECT_URI = process.env.NEXT_PUBLIC_REDIRECT_URI || 'https://localhost:3000/auth/callback';
const LOGOUT_REDIRECT_URI = process.env.NEXT_PUBLIC_LOGOUT_REDIRECT_URI || 'https://localhost:3000/';
const SCOPES = 'openid profile email roles api chamados offline_access';

// --- PKCE Helpers ---
function generateRandomString(length: number): string {
  const array = new Uint8Array(length);
  crypto.getRandomValues(array);
  return base64UrlEncode(array);
}

async function sha256(plain: string): Promise<ArrayBuffer> {
  const encoder = new TextEncoder();
  const data = encoder.encode(plain);
  return crypto.subtle.digest('SHA-256', data);
}

function base64UrlEncode(buffer: ArrayBuffer | Uint8Array): string {
  const bytes = buffer instanceof Uint8Array ? buffer : new Uint8Array(buffer);
  return btoa(String.fromCharCode(...bytes))
    .replace(/\+/g, '-')
    .replace(/\//g, '_')
    .replace(/=+$/, '');
}

// --- Auth Functions ---
export function initiateLogin(): void {
  const codeVerifier = generateRandomString(64);
  const state = generateRandomString(32);

  // Store in localStorage
  localStorage.setItem('pkce_code_verifier', codeVerifier);
  localStorage.setItem('pkce_state', state);

  // Build authorize URL (code_challenge will be set after async hash)
  const url = new URL(`${AUTH_SERVER_URL}/connect/authorize`);
  url.searchParams.set('client_id', CLIENT_ID);
  url.searchParams.set('redirect_uri', REDIRECT_URI);
  url.searchParams.set('response_type', 'code');
  url.searchParams.set('scope', SCOPES);
  url.searchParams.set('code_challenge_method', 'S256');
  url.searchParams.set('state', state);

  // Generate code_challenge synchronously-ish
  sha256(codeVerifier).then(hash => {
    const challenge = base64UrlEncode(hash);
    url.searchParams.set('code_challenge', challenge);
    window.location.href = url.toString();
  });
}

export async function handleCallback(code: string, state: string): Promise<boolean> {
  // Validate state (CSRF protection)
  const expectedState = localStorage.getItem('pkce_state');
  if (!expectedState || expectedState !== state) {
    console.error('State mismatch — possible CSRF attack');
    localStorage.removeItem('pkce_code_verifier');
    localStorage.removeItem('pkce_state');
    return false;
  }

  const codeVerifier = localStorage.getItem('pkce_code_verifier');
  if (!codeVerifier) {
    console.error('Code verifier not found in localStorage');
    return false;
  }

  // Exchange code for tokens
  const body = new URLSearchParams({
    grant_type: 'authorization_code',
    client_id: CLIENT_ID,
    code,
    redirect_uri: REDIRECT_URI,
    code_verifier: codeVerifier,
  });

  const response = await fetch(`${AUTH_SERVER_URL}/connect/token`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
    body,
  });

  if (!response.ok) {
    const error = await response.text();
    console.error('Token exchange failed:', error);
    return false;
  }

  const data = await response.json();
  if (data.access_token) {
    localStorage.setItem('authToken', data.access_token);
    if (data.refresh_token) {
      localStorage.setItem('refreshToken', data.refresh_token);
    }
    // Cleanup PKCE artifacts
    localStorage.removeItem('pkce_code_verifier');
    localStorage.removeItem('pkce_state');
    return true;
  }

  return false;
}

export function initiateLogout(): void {
  // Clear local tokens
  localStorage.removeItem('authToken');
  localStorage.removeItem('refreshToken');

  // Redirect to Auth Server logout
  const url = new URL(`${AUTH_SERVER_URL}/connect/logout`);
  url.searchParams.set('post_logout_redirect_uri', LOGOUT_REDIRECT_URI);
  window.location.href = url.toString();
}

export function getToken(): string | null {
  if (typeof window === 'undefined') return null;
  return localStorage.getItem('authToken');
}

export function getUser(): UserInfo | null {
  const token = getToken();
  if (!token) return null;

  try {
    // Decode JWT payload (no signature verification — same as Blazor client)
    const payload = token.split('.')[1];
    const decoded = JSON.parse(atob(payload.replace(/-/g, '+').replace(/_/g, '/')));

    // Check expiration
    if (decoded.exp && decoded.exp * 1000 < Date.now()) {
      localStorage.removeItem('authToken');
      localStorage.removeItem('refreshToken');
      return null;
    }

    // Extract roles (can be string or array)
    let roles: string[] = [];
    if (decoded.role) {
      roles = Array.isArray(decoded.role) ? decoded.role : [decoded.role];
    }

    return {
      isAuthenticated: true,
      userName: decoded.preferred_username || decoded.unique_name || decoded.name || '',
      email: decoded.email || '',
      userId: decoded.sub || '',
      roles,
    };
  } catch (e) {
    console.error('Failed to parse JWT:', e);
    return null;
  }
}

export function isAuthenticated(): boolean {
  return getToken() !== null && getUser() !== null;
}

export function hasRole(role: string): boolean {
  const user = getUser();
  return user?.roles.includes(role) ?? false;
}
