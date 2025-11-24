#!/usr/bin/env python3
"""
Script para testar autenticação OpenIddict
Uso: python test-openiddict-login.py
"""

import requests
import json
import base64
from datetime import datetime
from urllib3.exceptions import InsecureRequestWarning

# Desabilitar warnings de SSL para desenvolvimento
requests.packages.urllib3.disable_warnings(category=InsecureRequestWarning)

# Configuração
AUTH_SERVER = "https://localhost:7132"
API_SERVER = "https://localhost:7296"
USERNAME = "admin@admin.com"
PASSWORD = "Admin@123"
CLIENT_ID = "mobile-client"

# Cores para terminal
class Colors:
    GREEN = '\033[92m'
    RED = '\033[91m'
    YELLOW = '\033[93m'
    CYAN = '\033[96m'
    END = '\033[0m'

def print_header(text):
    print(f"\n{Colors.CYAN}{'=' * 50}{Colors.END}")
    print(f"{Colors.CYAN}{text.center(50)}{Colors.END}")
    print(f"{Colors.CYAN}{'=' * 50}{Colors.END}\n")

def print_success(text):
    print(f"{Colors.GREEN}✅ {text}{Colors.END}")

def print_error(text):
    print(f"{Colors.RED}❌ {text}{Colors.END}")

def print_info(text):
    print(f"{Colors.YELLOW}{text}{Colors.END}")

def decode_jwt_payload(token):
    """Decodifica o payload de um JWT"""
    try:
        parts = token.split('.')
        if len(parts) < 2:
            return None

        payload = parts[1]
        # Adicionar padding se necessário
        padding = 4 - (len(payload) % 4)
        if padding != 4:
            payload += '=' * padding

        decoded = base64.urlsafe_b64decode(payload)
        return json.loads(decoded)
    except Exception as e:
        print_error(f"Erro ao decodificar JWT: {e}")
        return None

def get_access_token():
    """Obtém access token usando Password Grant"""
    print_info("1. Obtendo token de acesso...")

    data = {
        'grant_type': 'password',
        'username': USERNAME,
        'password': PASSWORD,
        'client_id': CLIENT_ID,
        'scope': 'openid profile email roles api chamados'
    }

    try:
        response = requests.post(
            f"{AUTH_SERVER}/connect/token",
            data=data,
            verify=False  # Apenas para desenvolvimento
        )

        response.raise_for_status()
        token_data = response.json()

        if 'access_token' not in token_data:
            print_error("Token não encontrado na resposta")
            print(json.dumps(token_data, indent=2))
            return None

        print_success("Token obtido com sucesso!")
        print(f"\nAccess Token (primeiros 100 caracteres):")
        print(f"{token_data['access_token'][:100]}...")
        print(f"\nExpira em: {token_data.get('expires_in', 'N/A')} segundos")

        return token_data

    except requests.exceptions.RequestException as e:
        print_error(f"Erro ao obter token: {e}")
        if hasattr(e.response, 'text'):
            print(e.response.text)
        return None

def test_api_call(access_token):
    """Testa chamada à API com o token"""
    print_info("\n3. Testando chamada à API (GET /v1/categories)...")

    headers = {
        'Authorization': f'Bearer {access_token}',
        'Accept': 'application/json'
    }

    try:
        response = requests.get(
            f"{API_SERVER}/v1/categories",
            headers=headers,
            verify=False
        )

        response.raise_for_status()
        data = response.json()

        print_success("API respondeu com sucesso!")
        print("\nResposta:")
        print(json.dumps(data, indent=2, ensure_ascii=False))

        return True

    except requests.exceptions.RequestException as e:
        print_error(f"Erro na chamada à API: {e}")
        if hasattr(e.response, 'text'):
            print(e.response.text)
        return False

def test_refresh_token(refresh_token):
    """Testa renovação do token usando refresh token"""
    print_info("\n4. Testando Refresh Token...")

    data = {
        'grant_type': 'refresh_token',
        'refresh_token': refresh_token,
        'client_id': CLIENT_ID
    }

    try:
        response = requests.post(
            f"{AUTH_SERVER}/connect/token",
            data=data,
            verify=False
        )

        response.raise_for_status()
        token_data = response.json()

        if 'access_token' in token_data:
            print_success("Refresh token funcionou!")
            new_token = token_data['access_token']
            print(f"Novo access token (primeiros 100 caracteres): {new_token[:100]}...")
            return True
        else:
            print_error("Novo token não encontrado na resposta")
            return False

    except requests.exceptions.RequestException as e:
        print_error(f"Erro ao usar refresh token: {e}")
        if hasattr(e.response, 'text'):
            print(e.response.text)
        return False

def main():
    print_header("Teste de Autenticação OpenIddict")

    # 1. Obter token
    token_data = get_access_token()
    if not token_data:
        return 1

    access_token = token_data['access_token']
    refresh_token = token_data.get('refresh_token')

    # 2. Decodificar e mostrar payload
    print_info("\n2. Payload do token:")
    payload = decode_jwt_payload(access_token)
    if payload:
        print(json.dumps(payload, indent=2, ensure_ascii=False))

    # 3. Testar chamada à API
    if not test_api_call(access_token):
        return 1

    # 4. Salvar tokens
    tokens_file = '.tokens.json'
    tokens = {
        'access_token': access_token,
        'refresh_token': refresh_token,
        'expires_in': token_data.get('expires_in'),
        'obtained_at': datetime.now().isoformat()
    }

    with open(tokens_file, 'w') as f:
        json.dump(tokens, f, indent=2)

    print_info(f"\n💾 Tokens salvos em {tokens_file}")

    # 5. Testar refresh token
    if refresh_token:
        test_refresh_token(refresh_token)

    # Resultado final
    print_header("Teste concluído com sucesso!")

    print_info("\n📝 Para usar o token em outros requests (Python):")
    print(f"""
import requests

headers = {{
    'Authorization': 'Bearer {access_token[:50]}...',
    'Accept': 'application/json'
}}

response = requests.get(
    '{API_SERVER}/v1/categories',
    headers=headers,
    verify=False
)

print(response.json())
""")

    return 0

if __name__ == '__main__':
    exit(main())
