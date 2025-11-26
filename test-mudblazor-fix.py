#!/usr/bin/env python3
"""
Script para testar se as correções do MudBlazor funcionaram
"""

import requests
import time
import json


def test_get_order_types():
    """Testa se o endpoint de tipos de ordem está funcionando"""
    try:
        response = requests.get(
            "http://localhost:5071/v1/order-types",
            headers={"Accept": "application/json"},
            timeout=10,
        )
        print(f"GET /v1/order-types - Status: {response.status_code}")
        return response.status_code == 200
    except requests.exceptions.RequestException as e:
        print(f"Erro ao conectar no servidor: {e}")
        return False


def test_put_order_type():
    """Testa se o PUT endpoint foi corrigido (deve retornar 404, não 405)"""
    try:
        response = requests.put(
            "http://localhost:5071/v1/order-types/550e8400-e29b-41d4-a716-446655440000",
            headers={"Content-Type": "application/json"},
            json={"name": "Test Type Updated", "description": "Test Description"},
            timeout=10,
        )
        print(f"PUT /v1/order-types/{{id}} - Status: {response.status_code}")

        # 404 significa que o endpoint está funcionando (não encontrou o ID)
        # 405 (Method Not Allowed) significaria que o fix não funcionou
        if response.status_code == 404:
            print("✅ PUT endpoint funcionando (404 = endpoint ok, ID não encontrado)")
            return True
        elif response.status_code == 405:
            print("❌ PUT endpoint ainda retorna 405 (Method Not Allowed)")
            return False
        else:
            print(f"ℹ️ Status inesperado: {response.status_code}")
            return False

    except requests.exceptions.RequestException as e:
        print(f"Erro ao testar PUT: {e}")
        return False


def main():
    print("🔧 Testando correções do EChamado...")
    print("=" * 50)

    print("\n1. Testando GET de tipos de ordem...")
    get_ok = test_get_order_types()

    print("\n2. Testando PUT de tipos de ordem...")
    put_ok = test_put_order_type()

    print("\n" + "=" * 50)
    print("📊 RESULTADOS:")
    print(f"GET /order-types: {'✅ OK' if get_ok else '❌ FALHOU'}")
    print(f"PUT /order-types: {'✅ OK' if put_ok else '❌ FALHOU'}")

    if get_ok and put_ok:
        print("\n🎉 Todas as correções foram aplicadas com sucesso!")
        print("\n✅ Próximos passos:")
        print("1. Iniciar Auth Server (port 7132)")
        print("2. Iniciar API Server (port 7296)")
        print("3. Iniciar Client (port 7274)")
        print("4. Testar página /orders/create no navegador")
    else:
        print("\n⚠️ Algumas correções podem não ter funcionado")
        print("Verifique se o servidor está rodando na porta 5071")


if __name__ == "__main__":
    main()
