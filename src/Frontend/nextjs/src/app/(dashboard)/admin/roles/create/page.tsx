'use client';
import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { roleService } from '@/services';
import { ArrowLeft, Save } from 'lucide-react';
import Link from 'next/link';

export default function CreateRolePage() {
  const router = useRouter();
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [saving, setSaving] = useState(false);
  const handleSubmit = async (e: React.FormEvent) => { e.preventDefault(); if (!name.trim()) return; setSaving(true); try { const r = await roleService.create({ name, description, permissions: [] }); if (r.success) router.push('/admin/roles'); } finally { setSaving(false); } };
  return (
    <div className="max-w-2xl mx-auto">
      <div className="flex items-center gap-3 mb-6"><Link href="/admin/roles" className="text-gray-500 hover:text-gray-700"><ArrowLeft size={20} /></Link><h1 className="text-2xl font-bold text-gray-900 dark:text-white">Novo Perfil</h1></div>
      <form onSubmit={handleSubmit} className="bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-800 p-6 space-y-5">
        <div><label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Nome *</label><input type="text" value={name} onChange={e => setName(e.target.value)} maxLength={100} required className="w-full px-3 py-2 border border-gray-300 dark:border-gray-700 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white" /></div>
        <div><label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Descrição</label><textarea value={description} onChange={e => setDescription(e.target.value)} rows={3} className="w-full px-3 py-2 border border-gray-300 dark:border-gray-700 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white resize-none" /></div>
        <div className="flex gap-3 pt-4 border-t border-gray-200 dark:border-gray-700"><Link href="/admin/roles" className="px-4 py-2 border border-gray-300 dark:border-gray-700 rounded-lg">Cancelar</Link><button type="submit" disabled={saving} className="flex items-center gap-2 px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50"><Save size={16} /> {saving ? "Salvando..." : "Salvar"}</button></div>
      </form>
    </div>
  );
}
