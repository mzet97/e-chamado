# Script PowerShell para testar autenticação e API completa
# Execute este script no PowerShell

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "Teste Completo: Autenticação + API" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan

Write-Host ""
Write-Host "[1/4] Obtendo token do Auth Server (porta 7132)..." -ForegroundColor Yellow
Write-Host "----------------------------------------------" -ForegroundColor Gray

# Obter token
$tokenBody = @{
    grant_type = "password"
    username = "admin@admin.com"
    password = "Admin@123"
    client_id = "mobile-client"
    scope = "openid profile email roles api chamados"
}

try {
    $tokenResponse = Invoke-RestMethod -Uri "https://localhost:7133/connect/token" `
        -Method Post `
        -Body $tokenBody `
        -ContentType "application/x-www-form-urlencoded" `
        -SkipCertificateCheck

    if ($tokenResponse.access_token) {
        Write-Host "✅ Token obtido com sucesso" -ForegroundColor Green
        $accessToken = $tokenResponse.access_token

        Write-Host ""
        Write-Host "[2/4] Verificando token (primeiros 50 caracteres)..." -ForegroundColor Yellow
        Write-Host "----------------------------------------------" -ForegroundColor Gray
        Write-Host "Token: $($accessToken.Substring(0, [Math]::Min(50, $accessToken.Length)))..." -ForegroundColor White
        Write-Host "Expires in: $($tokenResponse.expires_in) segundos" -ForegroundColor White

        Write-Host ""
        Write-Host "[3/4] Testando API sem autenticação (deve retornar 401)..." -ForegroundColor Yellow
        Write-Host "----------------------------------------------" -ForegroundColor Gray

        try {
            $response = Invoke-WebRequest -Uri "https://localhost:7296/v1/category" `
                -Method Post `
                -Body '{"name": "Teste Sem Auth", "description": "Teste"}' `
                -ContentType "application/json" `
                -SkipCertificateCheck `
                -ErrorAction Stop

            Write-Host "❌ Retornou HTTP $($response.StatusCode) (esperado: 401)" -ForegroundColor Red
        } catch {
            if ($_.Exception.Response.StatusCode -eq 401) {
                Write-Host "✅ Retornou 401 Unauthorized (correto)" -ForegroundColor Green
            } else {
                Write-Host "❌ Retornou HTTP $($_.Exception.Response.StatusCode) (esperado: 401)" -ForegroundColor Red
            }
        }

        Write-Host ""
        Write-Host "[4/4] Testando API COM token Bearer..." -ForegroundColor Yellow
        Write-Host "----------------------------------------------" -ForegroundColor Gray

        try {
            $headers = @{
                "Authorization" = "Bearer $accessToken"
                "Content-Type" = "application/json"
            }

            $apiResponse = Invoke-WebRequest -Uri "https://localhost:7296/v1/category" `
                -Method Post `
                -Headers $headers `
                -Body '{"name": "Teste Com Token", "description": "Teste de autenticação"}' `
                -SkipCertificateCheck `
                -ErrorAction Stop

            Write-Host "HTTP Status: $($apiResponse.StatusCode)" -ForegroundColor White
            Write-Host "Response Body:" -ForegroundColor White
            Write-Host $apiResponse.Content -ForegroundColor White

            if ($apiResponse.StatusCode -eq 200 -or $apiResponse.StatusCode -eq 201) {
                Write-Host ""
                Write-Host "==========================================" -ForegroundColor Green
                Write-Host "✅ SUCESSO! API funcionando com token" -ForegroundColor Green
                Write-Host "==========================================" -ForegroundColor Green
            }
        } catch {
            $statusCode = $_.Exception.Response.StatusCode.value__
            Write-Host "HTTP Status: $statusCode" -ForegroundColor Red

            # Tentar ler o corpo da resposta
            $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
            $responseBody = $reader.ReadToEnd()
            $reader.Close()

            Write-Host "Response Body:" -ForegroundColor Red
            if ($responseBody) {
                Write-Host $responseBody -ForegroundColor Red
            }

            if ($statusCode -eq 401) {
                Write-Host ""
                Write-Host "==========================================" -ForegroundColor Red
                Write-Host "❌ ERRO 401: Token rejeitado" -ForegroundColor Red
                Write-Host "==========================================" -ForegroundColor Red
                Write-Host ""
                Write-Host "Possíveis causas:" -ForegroundColor Yellow
                Write-Host "1. EChamado.Server não foi reconstruído após mudança no IdentityConfig.cs" -ForegroundColor White
                Write-Host "2. Issuer não corresponde (deve ser: https://localhost:7133)" -ForegroundColor White
                Write-Host "3. Chaves de assinatura não correspondem" -ForegroundColor White
                Write-Host "4. Token expirou" -ForegroundColor White
                Write-Host ""
                Write-Host "Solução:" -ForegroundColor Yellow
                Write-Host "  cd E:\TI\git\e-chamado\src\EChamado\Server\EChamado.Server" -ForegroundColor White
                Write-Host "  .\rebuild-windows.ps1" -ForegroundColor White
            } else {
                Write-Host ""
                Write-Host "==========================================" -ForegroundColor Red
                Write-Host "❌ ERRO HTTP $statusCode" -ForegroundColor Red
                Write-Host "==========================================" -ForegroundColor Red
            }
        }

    } else {
        Write-Host "❌ Resposta não contém access_token" -ForegroundColor Red
        Write-Host ($tokenResponse | ConvertTo-Json) -ForegroundColor Red
    }
} catch {
    Write-Host "❌ Erro ao obter token" -ForegroundColor Red
    Write-Host "Mensagem: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "Verifique se o Echamado.Auth está rodando em https://localhost:7133" -ForegroundColor Yellow
}

Write-Host ""
