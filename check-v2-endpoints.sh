#!/bin/bash
# Script para verificar a compilação do projeto após implementação dos endpoints V2

echo "================================"
echo "Verificando implementação V2"
echo "================================"
echo ""

# Contar arquivos V2 criados
V2_COUNT=$(find /mnt/d/TI/git/e-chamado/src/EChamado/Server/EChamado.Server/Endpoints -name "*V2*.cs" -type f | wc -l)
echo "📊 Total de arquivos V2 encontrados: $V2_COUNT"
echo ""

# Listar arquivos V2
echo "📁 Arquivos V2 criados:"
find /mnt/d/TI/git/e-chamado/src/EChamado/Server/EChamado.Server/Endpoints -name "*V2*.cs" -type f | sort | while read file; do
    echo "  ✅ $file"
done
echo ""

# Verificar se todos os endpoints V2 estão configurados no Endpoint.cs
echo "🔍 Verificando Endpoint.cs..."
ENDPOINTS_V2_COUNT=$(grep -c "v2/" /mnt/d/TI/git/e-chamado/src/EChamado/Server/EChamado.Server/Endpoints/Endpoint.cs)
echo "  Linhas com v2/: $ENDPOINTS_V2_COUNT"
echo ""

echo "✅ Verificação concluída!"
echo ""
echo "Próximos passos:"
echo "1. Executar build do projeto: dotnet build"
echo "2. Verificar se todas as queries/commands existem"
echo "3. Executar testes de integração"
echo "4. Testar endpoints via Swagger UI"