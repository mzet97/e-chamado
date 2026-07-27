'use client';

import { useEffect, useState, useCallback } from 'react';
import { useSearchParams } from 'next/navigation';
import { useAuth } from '@/contexts/AuthContext';
import { orderService, lookupService } from '@/services';
import { OrderListViewModel, StatusTypeResponse, DepartmentResponse, OrderTypeResponse, SearchOrdersParameters } from '@/types/api';
import Link from 'next/link';
import { Search, Filter, ChevronLeft, ChevronRight, Plus, X } from 'lucide-react';
import { Suspense } from 'react';

function OrderListContent() {
  const { user } = useAuth();
  const searchParams = useSearchParams();
  const assignedOnly = searchParams.get('assigned') === 'true';

  const [orders, setOrders] = useState<OrderListViewModel[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(10);

  // Filters
  const [searchText, setSearchText] = useState('');
  const [selectedStatus, setSelectedStatus] = useState('');
  const [selectedDept, setSelectedDept] = useState('');
  const [selectedType, setSelectedType] = useState('');
  const [showFilters, setShowFilters] = useState(false);

  // Lookups
  const [statuses, setStatuses] = useState<StatusTypeResponse[]>([]);
  const [departments, setDepartments] = useState<DepartmentResponse[]>([]);
  const [types, setTypes] = useState<OrderTypeResponse[]>([]);

  useEffect(() => {
    Promise.all([
      lookupService.getStatusTypes(),
      lookupService.getDepartments(),
      lookupService.getOrderTypes(),
    ]).then(([s, d, t]) => { setStatuses(s); setDepartments(d); setTypes(t); });
  }, []);

  const loadOrders = useCallback(async () => {
    setLoading(true);
    try {
      const params: SearchOrdersParameters = {
        pageIndex: page,
        pageSize,
        title: searchText || undefined,
        statusTypeId: selectedStatus || undefined,
        departmentId: selectedDept || undefined,
        typeId: selectedType || undefined,
        assignedToUserId: assignedOnly && user ? user.userId : undefined,
      };
      const result = await orderService.search(params);
      if (result.success) {
        setOrders(result.data);
        setTotalCount(result.pagedResult.rowCount);
      }
    } catch (e) {
      console.error('Load orders error:', e);
    } finally {
      setLoading(false);
    }
  }, [page, pageSize, searchText, selectedStatus, selectedDept, selectedType, assignedOnly, user]);

  useEffect(() => { loadOrders(); }, [loadOrders]);

  const totalPages = Math.ceil(totalCount / pageSize);

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold text-gray-900 dark:text-white">
          {assignedOnly ? 'Meus Atribuídos' : 'Chamados'}
        </h1>
        <Link href="/orders/create" className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700">
          <Plus size={16} /> Novo Chamado
        </Link>
      </div>

      {/* Search + Filters */}
      <div className="bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-800 p-4">
        <div className="flex gap-3">
          <div className="flex-1 relative">
            <Search size={16} className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
            <input
              type="text"
              placeholder="Buscar por título..."
              value={searchText}
              onChange={e => setSearchText(e.target.value)}
              onKeyDown={e => e.key === 'Enter' && loadOrders()}
              className="w-full pl-10 pr-4 py-2 border border-gray-300 dark:border-gray-700 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white"
            />
          </div>
          <button onClick={() => setShowFilters(!showFilters)} className="flex items-center gap-2 px-4 py-2 border border-gray-300 dark:border-gray-700 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-800">
            <Filter size={16} /> Filtros
          </button>
        </div>

        {showFilters && (
          <div className="grid grid-cols-1 sm:grid-cols-3 gap-3 mt-3 pt-3 border-t border-gray-200 dark:border-gray-700">
            <select value={selectedStatus} onChange={e => setSelectedStatus(e.target.value)} className="px-3 py-2 border border-gray-300 dark:border-gray-700 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white">
              <option value="">Todos os Status</option>
              {statuses.map(s => <option key={s.id} value={s.id}>{s.name}</option>)}
            </select>
            <select value={selectedDept} onChange={e => setSelectedDept(e.target.value)} className="px-3 py-2 border border-gray-300 dark:border-gray-700 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white">
              <option value="">Todos os Departamentos</option>
              {departments.map(d => <option key={d.id} value={d.id}>{d.name}</option>)}
            </select>
            <select value={selectedType} onChange={e => setSelectedType(e.target.value)} className="px-3 py-2 border border-gray-300 dark:border-gray-700 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white">
              <option value="">Todos os Tipos</option>
              {types.map(t => <option key={t.id} value={t.id}>{t.name}</option>)}
            </select>
          </div>
        )}
      </div>

      {/* Table */}
      <div className="bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-800 overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b border-gray-200 dark:border-gray-800 bg-gray-50 dark:bg-gray-800/50">
                <th className="text-left p-4 font-medium text-gray-500 dark:text-gray-400">Título</th>
                <th className="text-left p-4 font-medium text-gray-500 dark:text-gray-400">Status</th>
                <th className="text-left p-4 font-medium text-gray-500 dark:text-gray-400">Tipo</th>
                <th className="text-left p-4 font-medium text-gray-500 dark:text-gray-400">Departamento</th>
                <th className="text-left p-4 font-medium text-gray-500 dark:text-gray-400">Responsável</th>
                <th className="text-left p-4 font-medium text-gray-500 dark:text-gray-400">Data</th>
                <th className="text-left p-4 font-medium text-gray-500 dark:text-gray-400">Ações</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                Array.from({ length: 5 }).map((_, i) => (
                  <tr key={i} className="border-b border-gray-100 dark:border-gray-800">
                    <td colSpan={7} className="p-4"><div className="h-4 bg-gray-200 dark:bg-gray-700 rounded animate-pulse" /></td>
                  </tr>
                ))
              ) : orders.length === 0 ? (
                <tr><td colSpan={7} className="p-8 text-center text-gray-500">Nenhum chamado encontrado</td></tr>
              ) : orders.map(order => (
                <tr key={order.id} className="border-b border-gray-100 dark:border-gray-800 hover:bg-gray-50 dark:hover:bg-gray-800/50">
                  <td className="p-4">
                    <Link href={`/orders/${order.id}`} className="text-blue-600 hover:underline font-medium">{order.title}</Link>
                  </td>
                  <td className="p-4"><StatusChip status={order.statusName} /></td>
                  <td className="p-4 text-gray-600 dark:text-gray-400">{order.typeName}</td>
                  <td className="p-4 text-gray-600 dark:text-gray-400">{order.departmentName || '-'}</td>
                  <td className="p-4 text-gray-600 dark:text-gray-400 text-xs">{order.responsibleUserEmail || '-'}</td>
                  <td className="p-4 text-gray-500 text-xs">{new Date(order.openingDate).toLocaleDateString('pt-BR')}</td>
                  <td className="p-4 flex gap-2">
                    <Link href={`/orders/${order.id}`} className="text-blue-600 hover:underline text-xs">Ver</Link>
                    <Link href={`/orders/${order.id}/edit`} className="text-gray-600 hover:underline text-xs">Editar</Link>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        {/* Pagination */}
        {totalPages > 1 && (
          <div className="flex items-center justify-between p-4 border-t border-gray-200 dark:border-gray-800">
            <p className="text-sm text-gray-500">
              Mostrando {(page - 1) * pageSize + 1}-{Math.min(page * pageSize, totalCount)} de {totalCount}
            </p>
            <div className="flex gap-2">
              <button onClick={() => setPage(p => Math.max(1, p - 1))} disabled={page === 1}
                className="p-2 rounded-lg border border-gray-300 dark:border-gray-700 disabled:opacity-50 hover:bg-gray-50 dark:hover:bg-gray-800">
                <ChevronLeft size={16} />
              </button>
              <span className="px-3 py-2 text-sm">{page} / {totalPages}</span>
              <button onClick={() => setPage(p => Math.min(totalPages, p + 1))} disabled={page === totalPages}
                className="p-2 rounded-lg border border-gray-300 dark:border-gray-700 disabled:opacity-50 hover:bg-gray-50 dark:hover:bg-gray-800">
                <ChevronRight size={16} />
              </button>
            </div>
          </div>
        )}
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
  return <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${color}`}>{status}</span>;
}

export default function OrdersPage() {
  return <Suspense fallback={<div className="p-8 text-center">Carregando...</div>}><OrderListContent /></Suspense>;
}
