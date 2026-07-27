import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-callback',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="flex items-center justify-center min-h-screen">
      <div class="text-center" *ngIf="!error">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4"></div>
        <p class="text-gray-600">Processando autenticação...</p>
      </div>
      <div class="text-center" *ngIf="error">
        <p class="text-red-600 mb-4">{{ error }}</p>
        <a href="/auth/login" class="text-blue-600 hover:underline">Tentar novamente</a>
      </div>
    </div>
  `,
})
export class CallbackComponent implements OnInit {
  error: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService
  ) {}

  async ngOnInit(): Promise<void> {
    const code = this.route.snapshot.queryParamMap.get('code');
    const state = this.route.snapshot.queryParamMap.get('state');

    if (!code || !state) {
      this.error = 'Parâmetros de autenticação ausentes';
      return;
    }

    const success = await this.authService.handleCallback(code, state);
    if (success) {
      this.router.navigate(['/']);
    } else {
      this.error = 'Falha na autenticação. Tente novamente.';
    }
  }
}
