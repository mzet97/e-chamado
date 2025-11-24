# Script para testar autenticação OpenIddict no Windows
# Uso: .\test-openiddict-login.ps1

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Teste de Autenticação OpenIddict" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# Configuração
$authServer = "https://localhost:7133"
$apiServer = "https://localhost:7296"
$username = "admin@admin.com"
$password = "Admin@123"
$clientId = "mobile-client"

# Ignorar erros de certificado SSL (apenas desenvolvimento)
if (-not ([System.Management.Automation.PSTypeName]'ServerCertificateValidationCallback').Type) {
    $certCallback = @"
        using System;
        using System.Net;
        using System.Net.Security;
        using System.Security.Cryptography.X509Certificates;
        public class ServerCertificateValidationCallback {
            public static void Ignore() {
                ServicePointManager.ServerCertificateValidationCallback +=
                    delegate (
                        Object obj,
                        X509Certificate certificate,
                        X509Chain chain,
                        SslPolicyErrors errors
                    ) {
                        return true;
                    };
            }
        }
"@
    Add-Type $certCallback
}
[ServerCertificateValidationCallback]::Ignore()

Write-Host "1. Obtendo token de acesso..." -ForegroundColor Yellow
Write-Host ""

# Fazer login
$body = @{
    grant_type = "password"
    username = $username
    password = $password
    client_id = $clientId
    scope = "openid profile email roles api chamados"
}

try {
    $tokenResponse = Invoke-RestMethod -Uri "$authServer/connect/token" `
        -Method Post `
        -ContentType "application/x-www-form-urlencoded" `
        -Body $body

    $accessToken = $tokenResponse.access_token
    $refreshToken = $tokenResponse.refresh_token
    $expiresIn = $tokenResponse.expires_in

    Write-Host "✅ Token obtido com sucesso!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Access Token (primeiros 100 caracteres):" -ForegroundColor Yellow
    Write-Host "$($accessToken.Substring(0, [Math]::Min(100, $accessToken.Length)))..."
    Write-Host ""
    Write-Host "Expira em: $expiresIn segundos" -ForegroundColor Yellow
    Write-Host ""

    # Decodificar payload do token
    Write-Host "2. Payload do token:" -ForegroundColor Yellow
    $tokenParts = $accessToken.Split('.')
    if ($tokenParts.Length -ge 2) {
        $payload = $tokenParts[1]
        # Adicionar padding se necessário
        $padding = $payload.Length % 4
        if ($padding -gt 0) {
            $payload += "=" * (4 - $padding)
        }

        try {
            $decodedBytes = [Convert]::FromBase64String($payload)
            $decodedJson = [System.Text.Encoding]::UTF8.GetString($decodedBytes)
            $payloadObject = $decodedJson | ConvertFrom-Json
            $payloadObject | ConvertTo-Json -Depth 10
        } catch {
            Write-Host "Não foi possível decodificar o payload" -ForegroundColor Red
        }
    }
    Write-Host ""

    # Testar chamada à API
    Write-Host "3. Testando chamada à API (GET /v1/categories)..." -ForegroundColor Yellow
    Write-Host ""

    $headers = @{
        "Authorization" = "Bearer $accessToken"
        "Accept" = "application/json"
    }

    $apiResponse = Invoke-RestMethod -Uri "$apiServer/v1/categories" `
        -Method Get `
        -Headers $headers

    Write-Host "✅ API respondeu com sucesso!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Resposta:" -ForegroundColor Yellow
    $apiResponse | ConvertTo-Json -Depth 10
    Write-Host ""

    # Salvar tokens
    $tokensObject = @{
        access_token = $accessToken
        refresh_token = $refreshToken
        expires_in = $expiresIn
        obtained_at = (Get-Date -Format "o")
    }

    $tokensObject | ConvertTo-Json | Out-File -FilePath ".tokens.json" -Encoding UTF8
    Write-Host "💾 Tokens salvos em .tokens.json" -ForegroundColor Yellow
    Write-Host ""

    # Testar refresh token
    if ($refreshToken) {
        Write-Host "4. Testando Refresh Token..." -ForegroundColor Yellow
        Write-Host ""

        $refreshBody = @{
            grant_type = "refresh_token"
            refresh_token = $refreshToken
            client_id = $clientId
        }

        try {
            $refreshResponse = Invoke-RestMethod -Uri "$authServer/connect/token" `
                -Method Post `
                -ContentType "application/x-www-form-urlencoded" `
                -Body $refreshBody

            Write-Host "✅ Refresh token funcionou!" -ForegroundColor Green
            $newAccessToken = $refreshResponse.access_token
            Write-Host "Novo access token (primeiros 100 caracteres): $($newAccessToken.Substring(0, [Math]::Min(100, $newAccessToken.Length)))..."
        } catch {
            Write-Host "❌ Erro ao usar refresh token:" -ForegroundColor Red
            Write-Host $_.Exception.Message
        }
    }

    Write-Host ""
    Write-Host "=========================================" -ForegroundColor Green
    Write-Host "✅ Teste concluído com sucesso!" -ForegroundColor Green
    Write-Host "=========================================" -ForegroundColor Green
    Write-Host ""

    Write-Host "📝 Para usar o token em outros requests:" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "`$headers = @{ `"Authorization`" = `"Bearer $accessToken`" }" -ForegroundColor Cyan
    Write-Host "Invoke-RestMethod -Uri `"$apiServer/v1/categories`" -Headers `$headers" -ForegroundColor Cyan
    Write-Host ""

} catch {
    Write-Host "❌ Erro ao obter token:" -ForegroundColor Red
    Write-Host $_.Exception.Message
    if ($_.ErrorDetails.Message) {
        Write-Host $_.ErrorDetails.Message
    }
    exit 1
}
