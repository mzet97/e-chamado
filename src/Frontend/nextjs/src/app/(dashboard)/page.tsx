'use client';

import { useEffect, useState } from 'react';
import { useAuth } from '@/contexts/AuthContext';
import { orderService } from '@/services';
import { OrderListViewModel, DashboardStatsResponse } from '@/types/api';
import { Ticket, Clock, AlertTriangle, CheckCircle, Plus, List } from 'lucide-react';
import Link from 'next/link';

export default function DashboardPage() {
  const { user } = useAuth();
  const [stats, setStats] = useState<DashboardStatsResponse | null>(null);
  const [recentOrders, setRecentOrders] = useState<OrderListViewModel[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function load() {
      try {
        const [statsRes, ordersRes] = await Promise.all([
          orderService.getDashboardStats(user?.userId),
          orderService.search({ pageIndex: 1, pageSize: 10 }),
        ]);
        if (statsRes.success) setStats(statsRes.data);
        if (ordersRes.success) setRecentOrders(ordersRes.data);
      } catch (e) {
        console.error('Dashboard load error:', e);
      } finally {
        setLoading(false);
      }
    }
    load();
  }, [user]);

  const kpiCards = [
    { label: 'Total de Chamados', value: stats?.totalTickets ?? 0, icon: <Ticket size={24} />, color: 'bg-blue-500' },
    { label: 'Meus Chamados', value: stats?.myTickets ?? 0, icon: <List size={24} />, color: 'bg-indigo-500' },
    { label: 'Atribuídos a Mim', value: stats?.assignedToMe ?? 0, icon: <Clock size={24} />, color: 'bg-amber-500' },
    { label: 'Em Atraso', value: stats?.overdueTickets ?? 0, icon: <AlertTriangle size={24} />, color: 'bg-red-500' },
  ];

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900 dark:text-white">Dashboard</h1>
          <p className="text-gray-500 dark:text-gray-400">Bem-vindo, {user?.userName || 'Usuário'}</p>
        </div>
        <Link
          href="/orders/create"
          className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
        >
          <Plus size={16} />
          Novo Chamado
        </Link>
      </div>

      {/* KPI Cards */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        {kpiCards.map((card, i) => (
          <div key={i} className="bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-800 p-5 flex items-center gap-4">
            <div className={`${card.color} p-3 rounded-lg text-white`}>{card.icon}</div>
            <div>
              <p className="text-sm text-gray-500 dark:text-gray-400">{card.label}</p>
              <p className="text-2xl font-bold text-gray-900 dark:text-white">
                {loading ? '...' : card.value}
              </p>
            </div>
          </div>
        ))}
      </div>

      {/* Recent Orders */}
      <div className="bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-800">
        <div className="p-5 border-b border-gray-200 dark:border-gray-800 flex items-center justify-between">
          <h2 className="text-lg font-semibold text-gray-900 dark:text-white">Últimos Chamados</h2>
          <Link href="/orders" className="text-sm text-blue-600 hover:underline">Ver todos →</Link>
        </div>
        <div className="overflow-x-auto">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b border-gray-200 dark:border-gray-800">
                <th className="text-left p-4 text-gray-500 dark:text-gray-400 font-medium">Título</th>
                <th className="text-left p-4 text-gray-500 dark:text-gray-400 font-medium">Status</th>
                <th className="text-left p-4 text-gray-500 dark:text-gray-400 font-medium">Tipo</th>
                <th className="text-left p-4 text-gray-500 dark:text-gray-400 font-medium">Departamento</th>
                <th className="text-left p-4 text-gray-500 dark:text-gray-400 font-medium">Data</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                Array.from({ length: 5 }).map((_, i) => (
                  <tr key={i} className="border-b border-gray-100 dark:border-gray-800">
                    <td className="p-4"><div className="h-4 bg-gray-200 dark:bg-gray-700 rounded w-48 animate-pulse" /></td>
                    <td className="p-4"><div className="h-6 bg-gray-200 dark:bg-gray-700 rounded w-20 animate-pulse" /></td>
                    <td className="p-4"><div className="h-4 bg-gray-200 dark:bg-gray-700 rounded w-24 animate-pulse" /></td>
                    <td className="p-4"><div className="h-4 bg-gray-200 dark:bg-gray-700 rounded w-28 animate-pulse" /></td>
                    <td className="p-4"><div className="h-4 bg-gray-200 dark:bg-gray-700 rounded w-20 animate-pulse" /></td>
                  </tr>
                ))
              ) : recentOrders.length === 0 ? (
                <tr>
                  <td colSpan={5} className="p-8 text-center text-gray-500 dark:text-gray-400">
                    <CheckCircle size={48} className="mx-auto mb-2 text-green-500" />
                    Nenhum chamado encontrado
                  </td>
                </tr>
              ) : (
                recentOrders.map(order => (
                  <tr key={order.id} className="border-b border-gray-100 dark:border-gray-800 hover:bg-gray-50 dark:hover:bg-gray-800/50">
                    <td className="p-4">
                      <Link href={`/orders/${order.id}`} className="text-blue-600 hover:underline font-medium">
                        {order.title}
                      </Link>
                    </td>
                    <td className="p-4">
                      <StatusChip status={order.statusName} />
                    </td>
                    <td className="p-4 text-gray-600 dark:text-gray-400">{order.typeName}</td>
                    <td className="p-4 text-gray-600 dark:text-gray-400">{order.departmentName || '-'}</td>
                    <td className="p-4 text-gray-500 dark:text-gray-400 text-xs">
                      {new Date(order.openingDate).toLocaleDateString('pt-BR')}
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>

      {/* Quick Actions */}
      <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
        <Link href="/orders/create" className="bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-800 p-5 hover:border-blue-300 dark:hover:border-blue-700 transition-colors">
          <Plus size={24} className="text-blue-600 mb-2" />
          <h3 className="font-semibold text-gray-900 dark:text-white">Novo Chamado</h3>
          <p className="text-sm text-gray-500 dark:text-gray-400">Criar uma nova solicitação</p>
        </Link>
        <Link href="/orders?assigned=true" className="bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-800 p-5 hover:border-blue-300 dark:hover:border-blue-700 transition-colors">
          <Clock size={24} className="text-amber-600 mb-2" />
          <h3 className="font-semibold text-gray-900 dark:text-white">Meus Atribuídos</h3>
          <p className="text-sm text-gray-500 dark:text-gray-400">Chamados atribuídos a mim</p>
        </Link>
        <Link href="/orders" className="bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-800 p-5 hover:border-blue-300 dark:hover:border-blue-700 transition-colors">
          <List size={24} className="text-indigo-600 mb-2" />
          <h3 className="font-semibold text-gray-900 dark:text-white">Listar Todos</h3>
          <p className="text-sm text-gray-500 dark:text-gray-400">Ver todos os chamados</p>
        </Link>
      </div>
    </div>
  );
}

function StatusChip({ status }: { status: string }) {
  const s = status?.toLowerCase() || '';
  let color = 'bg-gray-100 text-gray-700 dark:bg-gray-800 dark:text-gray-300';
  if (s.includes('aberto') || s.includes('open')) color = 'bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400';
  else if (s.includes('andamento') || s.includes('progress')) color = 'bg-amber-100 text-amber-700 dark:bg-amber-900/30 dark:text-amber-400';
  else if (s.includes('fechado') || s.includes('closed') || s.includes('resolvido')) color = 'bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400';

  return (
    <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${color}`}>
      {status}
    </span>
  );
}
