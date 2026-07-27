'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { useAuth } from '@/contexts/AuthContext';
import { orderService, lookupService } from '@/services';
import { OrderTypeResponse, DepartmentResponse, CategoryResponse, SubCategoryResponse } from '@/types/api';
import { ArrowLeft, Save } from 'lucide-react';
import Link from 'next/link';

export default function CreateOrderPage() {
  const router = useRouter();
  const { user } = useAuth();

  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [typeId, setTypeId] = useState('');
  const [departmentId, setDepartmentId] = useState('');
  const [categoryId, setCategoryId] = useState('');
  const [subCategoryId, setSubCategoryId] = useState('');
  const [dueDate, setDueDate] = useState('');

  const [types, setTypes] = useState<OrderTypeResponse[]>([]);
  const [departments, setDepartments] = useState<DepartmentResponse[]>([]);
  const [categories, setCategories] = useState<CategoryResponse[]>([]);
  const [subCategories, setSubCategories] = useState<SubCategoryResponse[]>([]);

  const [saving, setSaving] = useState(false);
  const [errors, setErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    Promise.all([
      lookupService.getOrderTypes(),
      lookupService.getDepartments(),
      lookupService.getCategories(),
    ]).then(([t, d, c]) => { setTypes(t); setDepartments(d); setCategories(c); });
  }, []);

  useEffect(() => {
    if (categoryId) {
      lookupService.getSubCategories(categoryId).then(setSubCategories);
      setSubCategoryId('');
    } else {
      setSubCategories([]);
    }
  }, [categoryId]);

  const validate = () => {
    const e: Record<string, string> = {};
    if (!title.trim()) e.title = 'Título é obrigatório';
    if (title.length > 200) e.title = 'Máximo 200 caracteres';
    if (!description.trim()) e.description = 'Descrição é obrigatória';
    if (description.length > 2000) e.description = 'Máximo 2000 caracteres';
    if (!typeId) e.typeId = 'Tipo é obrigatório';
    if (!categoryId) e.categoryId = 'Categoria é obrigatória';
    if (!departmentId) e.departmentId = 'Departamento é obrigatório';
    setErrors(e);
    return Object.keys(e).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate() || !user) return;

    setSaving(true);
    try {
      const result = await orderService.create({
        title,
        description,
        typeId,
        categoryId,
        departmentId,
        subCategoryId: subCategoryId || undefined,
        dueDate: dueDate || undefined,
        requestingUserId: user.userId,
        requestingUserEmail: user.email,
      });
      if (result.success) {
        router.push(`/orders/${result.data}`);
      }
    } catch (err) {
      console.error(err);
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="max-w-3xl mx-auto">
      <div className="flex items-center gap-3 mb-6">
        <Link href="/orders" className="text-gray-500 hover:text-gray-700"><ArrowLeft size={20} /></Link>
        <h1 className="text-2xl font-bold text-gray-900 dark:text-white">Novo Chamado</h1>
      </div>

      <form onSubmit={handleSubmit} className="bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-800 p-6 space-y-5">
        {/* Title */}
        <div>
          <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Título *</label>
          <input type="text" value={title} onChange={e => setTitle(e.target.value)} maxLength={200}
            className={`w-full px-3 py-2 border rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white ${errors.title ? 'border-red-500' : 'border-gray-300 dark:border-gray-700'}`} />
          <div className="flex justify-between mt-1">
            {errors.title && <p className="text-red-500 text-xs">{errors.title}</p>}
            <p className="text-xs text-gray-400 ml-auto">{title.length}/200</p>
          </div>
        </div>

        {/* Description */}
        <div>
          <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Descrição *</label>
          <textarea value={description} onChange={e => setDescription(e.target.value)} rows={5} maxLength={2000}
            className={`w-full px-3 py-2 border rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white resize-none ${errors.description ? 'border-red-500' : 'border-gray-300 dark:border-gray-700'}`} />
          <div className="flex justify-between mt-1">
            {errors.description && <p className="text-red-500 text-xs">{errors.description}</p>}
            <p className="text-xs text-gray-400 ml-auto">{description.length}/2000</p>
          </div>
        </div>

        {/* Type */}
        <div>
          <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Tipo *</label>
          <select value={typeId} onChange={e => setTypeId(e.target.value)}
            className={`w-full px-3 py-2 border rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white ${errors.typeId ? 'border-red-500' : 'border-gray-300 dark:border-gray-700'}`}>
            <option value="">Selecione...</option>
            {types.map(t => <option key={t.id} value={t.id}>{t.name}</option>)}
          </select>
          {errors.typeId && <p className="text-red-500 text-xs mt-1">{errors.typeId}</p>}
        </div>

        {/* Department */}
        <div>
          <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Departamento *</label>
          <select value={departmentId} onChange={e => setDepartmentId(e.target.value)}
            className={`w-full px-3 py-2 border rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white ${errors.departmentId ? 'border-red-500' : 'border-gray-300 dark:border-gray-700'}`}>
            <option value="">Selecione...</option>
            {departments.map(d => <option key={d.id} value={d.id}>{d.name}</option>)}
          </select>
          {errors.departmentId && <p className="text-red-500 text-xs mt-1">{errors.departmentId}</p>}
        </div>

        {/* Category + SubCategory */}
        <div className="grid grid-cols-2 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Categoria *</label>
            <select value={categoryId} onChange={e => setCategoryId(e.target.value)}
              className={`w-full px-3 py-2 border rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white ${errors.categoryId ? 'border-red-500' : 'border-gray-300 dark:border-gray-700'}`}>
              <option value="">Selecione...</option>
              {categories.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
            </select>
            {errors.categoryId && <p className="text-red-500 text-xs mt-1">{errors.categoryId}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Subcategoria</label>
            <select value={subCategoryId} onChange={e => setSubCategoryId(e.target.value)} disabled={!categoryId}
              className="w-full px-3 py-2 border border-gray-300 dark:border-gray-700 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white disabled:opacity-50">
              <option value="">Selecione...</option>
              {subCategories.map(s => <option key={s.id} value={s.id}>{s.name}</option>)}
            </select>
          </div>
        </div>

        {/* Due Date */}
        <div>
          <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Prazo</label>
          <input type="date" value={dueDate} onChange={e => setDueDate(e.target.value)}
            min={new Date().toISOString().split('T')[0]}
            className="w-full px-3 py-2 border border-gray-300 dark:border-gray-700 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white" />
        </div>

        {/* Actions */}
        <div className="flex gap-3 pt-4 border-t border-gray-200 dark:border-gray-700">
          <Link href="/orders" className="px-4 py-2 border border-gray-300 dark:border-gray-700 rounded-lg text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-800">
            Cancelar
          </Link>
          <button type="submit" disabled={saving}
            className="flex items-center gap-2 px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50">
            <Save size={16} />
            {saving ? 'Salvando...' : 'Criar Chamado'}
          </button>
        </div>
      </form>
    </div>
  );
}
