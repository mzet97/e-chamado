'use client';
import { useEffect, useState } from 'react';
import { lookupService } from '@/services';
import { OrderTypeResponse } from '@/types/api';
import { Plus, Edit, Trash2, Search } from 'lucide-react';
import Link from 'next/link';

export default function OrderTypesPage() {
  const [items, setItems] = useState<OrderTypeResponse[]>([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [deleting, setDeleting] = useState<string | null>(null);

  const load = async () => { setLoading(true); try { const r = await lookupService.getOrderTypes(true); setItems(r); } finally { setLoading(false); } };
  useEffect(() => { load(); }, []);

  const handleDelete = async (id: string, name: string) => {
    if (!confirm(`Excluir \"${name}\"?`)) return;
    setDeleting(id); try { await lookupService.deleteOrderType(id); await load(); } finally { setDeleting(null); }
  };

  const filtered = items.filter(d => d.name.toLowerCase().includes(search.toLowerCase()));

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold text-gray-900 dark:text-white">Tipos de Chamado</h1>
        <Link href="/admin/ordertypes/create" className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"><Plus size={16} /> Novo</Link>
      </div>
      <div className="bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-800">
        <div className="p-4 border-b border-gray-200 dark:border-gray-800">
          <div className="relative"><Search size={16} className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" /><input type="text" placeholder="Buscar..." value={search} onChange={e => setSearch(e.target.value)} className="w-full pl-10 pr-4 py-2 border border-gray-300 dark:border-gray-700 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white" /></div>
        </div>
        <div className="overflow-x-auto">
          <table className="w-full text-sm">
            <thead><tr className="border-b border-gray-200 dark:border-gray-800 bg-gray-50 dark:bg-gray-800/50"><th className="text-left p-4 font-medium text-gray-500">Nome</th><th className="text-left p-4 font-medium text-gray-500">Descrição</th><th className="text-right p-4 font-medium text-gray-500">Ações</th></tr></thead>
            <tbody>{loading ? <tr><td colSpan={3} className="p-4"><div className="h-4 bg-gray-200 dark:bg-gray-700 rounded animate-pulse" /></td></tr> :
              filtered.length === 0 ? <tr><td colSpan={3} className="p-8 text-center text-gray-500">Nenhum tipo encontrado</td></tr> :
              filtered.map(d => (
                <tr key={d.id} className="border-b border-gray-100 dark:border-gray-800 hover:bg-gray-50 dark:hover:bg-gray-800/50">
                  <td className="p-4 font-medium text-gray-900 dark:text-white">{d.name}</td>
                  <td className="p-4 text-gray-600 dark:text-gray-400">{d.description || '-'}</td>
                  <td className="p-4 text-right"><div className="flex justify-end gap-2">
                    <Link href={`/admin/ordertypes/${d.id}/edit`} className="p-2 text-blue-600 hover:bg-blue-50 dark:hover:bg-blue-900/20 rounded-lg"><Edit size={16} /></Link>
                    <button onClick={() => handleDelete(d.id, d.name)} disabled={deleting === d.id} className="p-2 text-red-600 hover:bg-red-50 dark:hover:bg-red-900/20 rounded-lg disabled:opacity-50"><Trash2 size={16} /></button>
                  </div></td>
                </tr>
              ))}</tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
