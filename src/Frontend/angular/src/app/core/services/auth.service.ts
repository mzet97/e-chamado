import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { UserInfo } from '../models/api.interfaces';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly AUTH_URL = environment.authUrl;
  private readonly CLIENT_ID = environment.clientId;
  private readonly REDIRECT_URI = environment.redirectUri;
  private readonly SCOPES = environment.scopes;

  // --- PKCE Helpers ---
  private generateRandomString(length: number): string {
    const array = new Uint8Array(length);
    crypto.getRandomValues(array);
    return this.base64UrlEncode(array);
  }

  private base64UrlEncode(buffer: ArrayBuffer | Uint8Array): string {
    const bytes = buffer instanceof Uint8Array ? buffer : new Uint8Array(buffer);
    return btoa(String.fromCharCode(...bytes)).replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '');
  }

  private async sha256(plain: string): Promise<ArrayBuffer> {
    const encoder = new TextEncoder();
    return crypto.subtle.digest('SHA-256', encoder.encode(plain));
  }

  // --- Auth Flow ---
  async login(): Promise<void> {
    const codeVerifier = this.generateRandomString(64);
    const state = this.generateRandomString(32);

    localStorage.setItem('pkce_code_verifier', codeVerifier);
    localStorage.setItem('pkce_state', state);

    const challenge = this.base64UrlEncode(await this.sha256(codeVerifier));
    const url = new URL(`${this.AUTH_URL}/connect/authorize`);
    url.searchParams.set('client_id', this.CLIENT_ID);
    url.searchParams.set('redirect_uri', this.REDIRECT_URI);
    url.searchParams.set('response_type', 'code');
    url.searchParams.set('scope', this.SCOPES);
    url.searchParams.set('code_challenge', challenge);
    url.searchParams.set('code_challenge_method', 'S256');
    url.searchParams.set('state', state);

    window.location.href = url.toString();
  }

  async handleCallback(code: string, state: string): Promise<boolean> {
    const expectedState = localStorage.getItem('pkce_state');
    if (!expectedState || expectedState !== state) {
      console.error('State mismatch — CSRF protection');
      this.cleanup();
      return false;
    }

    const codeVerifier = localStorage.getItem('pkce_code_verifier');
    if (!codeVerifier) return false;

    const body = new URLSearchParams({
      grant_type: 'authorization_code',
      client_id: this.CLIENT_ID,
      code,
      redirect_uri: this.REDIRECT_URI,
      code_verifier: codeVerifier,
    });

    const response = await fetch(`${this.AUTH_URL}/connect/token`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
      body,
    });

    if (!response.ok) return false;

    const data = await response.json();
    if (data.access_token) {
      localStorage.setItem('authToken', data.access_token);
      if (data.refresh_token) localStorage.setItem('refreshToken', data.refresh_token);
      this.cleanup();
      return true;
    }
    return false;
  }

  logout(): void {
    localStorage.removeItem('authToken');
    localStorage.removeItem('refreshToken');
    const url = new URL(`${this.AUTH_URL}/connect/logout`);
    url.searchParams.set('post_logout_redirect_uri', environment.logoutRedirectUri);
    window.location.href = url.toString();
  }

  getToken(): string | null {
    return localStorage.getItem('authToken');
  }

  getUser(): UserInfo | null {
    const token = this.getToken();
    if (!token) return null;

    try {
      const payload = token.split('.')[1];
      const decoded = JSON.parse(atob(payload.replace(/-/g, '+').replace(/_/g, '/')));

      if (decoded.exp && decoded.exp * 1000 < Date.now()) {
        this.clearTokens();
        return null;
      }

      let roles: string[] = [];
      if (decoded.role) roles = Array.isArray(decoded.role) ? decoded.role : [decoded.role];

      return {
        isAuthenticated: true,
        userName: decoded.preferred_username || decoded.unique_name || decoded.name || '',
        email: decoded.email || '',
        userId: decoded.sub || '',
        roles,
      };
    } catch {
      return null;
    }
  }

  isAuthenticated(): boolean {
    return this.getToken() !== null && this.getUser() !== null;
  }

  hasRole(role: string): boolean {
    return this.getUser()?.roles.includes(role) ?? false;
  }

  private clearTokens(): void {
    localStorage.removeItem('authToken');
    localStorage.removeItem('refreshToken');
  }

  private cleanup(): void {
    localStorage.removeItem('pkce_code_verifier');
    localStorage.removeItem('pkce_state');
  }
}
