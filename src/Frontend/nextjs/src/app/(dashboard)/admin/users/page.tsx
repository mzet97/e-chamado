'use client';
import { useEffect, useState } from 'react';
import { userService } from '@/services';
import { UserResponse } from '@/types/api';

export default function UsersPage() {
  const [users, setUsers] = useState<UserResponse[]>([]);
  const [loading, setLoading] = useState(true);
  useEffect(() => { userService.getAll().then(r => { if (r.success) setUsers(r.data); setLoading(false); }); }, []);
  return (
    <div className="space-y-4">
      <h1 className="text-2xl font-bold text-gray-900 dark:text-white">Usuários</h1>
      <div className="bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-800 overflow-x-auto">
        <table className="w-full text-sm">
          <thead><tr className="border-b border-gray-200 dark:border-gray-800 bg-gray-50 dark:bg-gray-800/50">
            <th className="text-left p-4 font-medium text-gray-500">Email</th><th className="text-left p-4 font-medium text-gray-500">Usuário</th><th className="text-left p-4 font-medium text-gray-500">Email OK</th><th className="text-left p-4 font-medium text-gray-500">2FA</th><th className="text-left p-4 font-medium text-gray-500">Status</th>
          </tr></thead>
          <tbody>{loading ? <tr><td colSpan={5} className="p-4"><div className="h-4 bg-gray-200 dark:bg-gray-700 rounded animate-pulse" /></td></tr> :
            users.map(u => (
              <tr key={u.id} className="border-b border-gray-100 dark:border-gray-800 hover:bg-gray-50 dark:hover:bg-gray-800/50">
                <td className="p-4 text-gray-900 dark:text-white">{u.email}</td>
                <td className="p-4 text-gray-600 dark:text-gray-400">{u.userName}</td>
                <td className="p-4">{u.emailConfirmed ? '✅' : '❌'}</td>
                <td className="p-4">{u.twoFactorEnabled ? '✅' : '-'}</td>
                <td className="p-4">{u.lockoutEnd && new Date(u.lockoutEnd) > new Date() ? '🔒 Bloqueado' : '✅ Ativo'}</td>
              </tr>
            ))}</tbody>
        </table>
      </div>
    </div>
  );
}
