import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-logout',
  standalone: true,
  template: `
    <div class="flex items-center justify-center min-h-screen">
      <div class="text-center">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4"></div>
        <p class="text-gray-600">Encerrando sessão...</p>
      </div>
    </div>
  `,
})
export class LogoutComponent implements OnInit {
  constructor(private authService: AuthService) {}
  ngOnInit(): void { this.authService.logout(); }
}
