'use client';

import { useEffect, useState } from 'react';
import { useParams, useRouter } from 'next/navigation';
import { useAuth } from '@/contexts/AuthContext';
import { orderService, lookupService } from '@/services';
import { OrderViewModel, CommentResponse, StatusTypeResponse } from '@/types/api';
import Link from 'next/link';
import { ArrowLeft, Edit, Send, Star, Lock, CheckCircle, UserCheck } from 'lucide-react';

export default function OrderDetailsPage() {
  const { id } = useParams<{ id: string }>();
  const router = useRouter();
  const { user } = useAuth();

  const [order, setOrder] = useState<OrderViewModel | null>(null);
  const [loading, setLoading] = useState(true);
  const [statuses, setStatuses] = useState<StatusTypeResponse[]>([]);
  const [selectedStatus, setSelectedStatus] = useState('');
  const [updatingStatus, setUpdatingStatus] = useState(false);

  // Comments
  const [newComment, setNewComment] = useState('');
  const [isInternal, setIsInternal] = useState(false);
  const [addingComment, setAddingComment] = useState(false);

  // Close dialog
  const [showCloseDialog, setShowCloseDialog] = useState(false);
  const [evaluation, setEvaluation] = useState(0);
  const [closing, setClosing] = useState(false);

  // Assign
  const [assigning, setAssigning] = useState(false);

  useEffect(() => {
    async function load() {
      try {
        const [orderRes, statusRes] = await Promise.all([
          orderService.getById(id),
          lookupService.getStatusTypes(),
        ]);
        if (orderRes.success) {
          setOrder(orderRes.data);
          setSelectedStatus(orderRes.data.statusId);
        }
        setStatuses(statusRes);
      } catch (e) {
        console.error(e);
      } finally {
        setLoading(false);
      }
    }
    load();
  }, [id]);

  const reload = async () => {
    const res = await orderService.getById(id);
    if (res.success) setOrder(res.data);
  };

  const handleAddComment = async () => {
    if (!newComment.trim() || !user) return;
    setAddingComment(true);
    try {
      await orderService.addComment({
        orderId: id,
        description: newComment,
        userId: user.userId,
        userEmail: user.email,
        isInternal,
      });
      setNewComment('');
      setIsInternal(false);
      await reload();
    } catch (e) {
      console.error(e);
    } finally {
      setAddingComment(false);
    }
  };

  const handleChangeStatus = async () => {
    if (!selectedStatus || selectedStatus === order?.statusId) return;
    setUpdatingStatus(true);
    try {
      await orderService.changeStatus({ orderId: id, statusTypeId: selectedStatus });
      await reload();
    } finally {
      setUpdatingStatus(false);
    }
  };

  const handleAssign = async () => {
    if (!user) return;
    setAssigning(true);
    try {
      await orderService.assign({ orderId: id, assignedToUserId: user.userId });
      await reload();
    } finally {
      setAssigning(false);
    }
  };

  const handleClose = async () => {
    setClosing(true);
    try {
      await orderService.close({ orderId: id, evaluation: evaluation > 0 ? evaluation : undefined });
      setShowCloseDialog(false);
      await reload();
    } finally {
      setClosing(false);
    }
  };

  if (loading) return <div className="p-8"><div className="animate-pulse space-y-4"><div className="h-8 bg-gray-200 dark:bg-gray-700 rounded w-1/3" /><div className="h-40 bg-gray-200 dark:bg-gray-700 rounded" /></div></div>;
  if (!order) return <div className="p-8"><p className="text-red-500">Chamado não encontrado</p></div>;

  const isOpen = !order.closingDate;

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <div className="flex items-center gap-3 mb-1">
            <Link href="/orders" className="text-gray-500 hover:text-gray-700"><ArrowLeft size={20} /></Link>
            <h1 className="text-2xl font-bold text-gray-900 dark:text-white">{order.title}</h1>
          </div>
          <p className="text-sm text-gray-500">Criado em {new Date(order.openingDate).toLocaleString('pt-BR')}</p>
        </div>
        <Link href={`/orders/${id}/edit`} className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700">
          <Edit size={16} /> Editar
        </Link>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Left column */}
        <div className="lg:col-span-2 space-y-6">
          {/* Description */}
          <div className="bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-800 p-5">
            <h2 className="text-lg font-semibold mb-3 text-gray-900 dark:text-white">Descrição</h2>
            <p className="text-gray-700 dark:text-gray-300 whitespace-pre-wrap">{order.description}</p>
          </div>

          {/* Comments */}
          <div className="bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-800 p-5">
            <h2 className="text-lg font-semibold mb-4 text-gray-900 dark:text-white">Comentários</h2>

            {order.comments?.length > 0 ? (
              <div className="space-y-4 mb-6">
                {order.comments.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()).map(c => (
                  <div key={c.id} className="flex gap-3">
                    <div className="w-8 h-8 bg-blue-100 dark:bg-blue-900/30 rounded-full flex items-center justify-center text-blue-600 text-xs font-bold">
                      {c.userEmail?.charAt(0)?.toUpperCase() || '?'}
                    </div>
                    <div className="flex-1 bg-gray-50 dark:bg-gray-800 rounded-lg p-3">
                      <div className="flex items-center gap-2 mb-1">
                        <span className="text-sm font-medium text-gray-900 dark:text-white">{c.userEmail}</span>
                        {c.isInternal && (
                          <span className="flex items-center gap-1 px-1.5 py-0.5 bg-amber-100 dark:bg-amber-900/30 text-amber-700 dark:text-amber-400 rounded text-xs">
                            <Lock size={10} /> Interno
                          </span>
                        )}
                        <span className="text-xs text-gray-500">{new Date(c.createdAt).toLocaleString('pt-BR')}</span>
                      </div>
                      <p className="text-sm text-gray-700 dark:text-gray-300 whitespace-pre-wrap">{c.text}</p>
                    </div>
                  </div>
                ))}
              </div>
            ) : (
              <p className="text-gray-500 text-center py-4">Nenhum comentário ainda</p>
            )}

            {/* Add comment */}
            <div className="border-t border-gray-200 dark:border-gray-700 pt-4">
              <div className="flex items-center gap-2 mb-2">
                <input type="checkbox" id="internal" checked={isInternal} onChange={e => setIsInternal(e.target.checked)} className="rounded" />
                <label htmlFor="internal" className="text-sm text-gray-600 dark:text-gray-400 flex items-center gap-1">
                  <Lock size={12} /> Comentário interno
                </label>
              </div>
              <div className="flex gap-2">
                <textarea
                  value={newComment}
                  onChange={e => setNewComment(e.target.value)}
                  placeholder="Adicionar comentário..."
                  rows={3}
                  className="flex-1 px-3 py-2 border border-gray-300 dark:border-gray-700 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white resize-none"
                />
                <button onClick={handleAddComment} disabled={!newComment.trim() || addingComment}
                  className="self-end px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50">
                  {addingComment ? '...' : <Send size={16} />}
                </button>
              </div>
            </div>
          </div>
        </div>

        {/* Right column — Details + Actions */}
        <div className="space-y-6">
          {/* Details */}
          <div className="bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-800 p-5">
            <h2 className="text-lg font-semibold mb-3 text-gray-900 dark:text-white">Detalhes</h2>
            <dl className="space-y-3 text-sm">
              <DetailRow label="Status"><StatusChip status={order.statusName} /></DetailRow>
              <DetailRow label="Tipo">{order.typeName}</DetailRow>
              <DetailRow label="Departamento">{order.departmentName || '-'}</DetailRow>
              <DetailRow label="Categoria">{order.categoryName || '-'}</DetailRow>
              {order.subCategoryName && <DetailRow label="Subcategoria">{order.subCategoryName}</DetailRow>}
              <DetailRow label="Prazo">
                {order.dueDate ? (
                  <span className={order.isOverdue ? 'text-red-600 font-medium' : ''}>
                    {new Date(order.dueDate).toLocaleDateString('pt-BR')}
                    {order.isOverdue && ' (Vencido)'}
                  </span>
                ) : '-'}
              </DetailRow>
              <DetailRow label="Solicitante">{order.requestingUserEmail}</DetailRow>
              <DetailRow label="Responsável">{order.responsibleUserEmail || 'Não atribuído'}</DetailRow>
              {order.closingDate && (
                <DetailRow label="Fechado em">
                  {new Date(order.closingDate).toLocaleString('pt-BR')}
                  {order.evaluation && ` — Avaliação: ${order.evaluation}/5`}
                </DetailRow>
              )}
            </dl>
          </div>

          {/* Actions */}
          <div className="bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-800 p-5">
            <h2 className="text-lg font-semibold mb-3 text-gray-900 dark:text-white">Ações</h2>
            <div className="space-y-3">
              {/* Change status */}
              <div>
                <label className="text-sm text-gray-600 dark:text-gray-400 mb-1 block">Alterar Status</label>
                <div className="flex gap-2">
                  <select value={selectedStatus} onChange={e => setSelectedStatus(e.target.value)}
                    className="flex-1 px-3 py-2 border border-gray-300 dark:border-gray-700 rounded-lg bg-white dark:bg-gray-800 text-gray-900 dark:text-white">
                    {statuses.map(s => <option key={s.id} value={s.id}>{s.name}</option>)}
                  </select>
                  <button onClick={handleChangeStatus} disabled={selectedStatus === order.statusId || updatingStatus}
                    className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50">
                    {updatingStatus ? '...' : 'Atualizar'}
                  </button>
                </div>
              </div>

              {/* Assign to me */}
              {(!order.responsibleUserEmail || order.responsibleUserEmail === '-') && (
                <button onClick={handleAssign} disabled={assigning}
                  className="w-full flex items-center justify-center gap-2 px-4 py-2 border border-blue-300 text-blue-600 rounded-lg hover:bg-blue-50 dark:hover:bg-blue-900/20 disabled:opacity-50">
                  <UserCheck size={16} />
                  {assigning ? 'Assumindo...' : 'Assumir Chamado'}
                </button>
              )}

              {/* Close */}
              {isOpen && (
                <button onClick={() => setShowCloseDialog(true)}
                  className="w-full flex items-center justify-center gap-2 px-4 py-2 border border-green-300 text-green-600 rounded-lg hover:bg-green-50 dark:hover:bg-green-900/20">
                  <CheckCircle size={16} /> Fechar Chamado
                </button>
              )}
            </div>
          </div>
        </div>
      </div>

      {/* Close Dialog */}
      {showCloseDialog && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50" onClick={() => setShowCloseDialog(false)}>
          <div className="bg-white dark:bg-gray-900 rounded-xl p-6 w-full max-w-md mx-4" onClick={e => e.stopPropagation()}>
            <h3 className="text-lg font-semibold mb-4 text-gray-900 dark:text-white">Fechar Chamado</h3>
            <p className="text-sm text-gray-600 dark:text-gray-400 mb-4">Avalie o atendimento (opcional):</p>
            <div className="flex justify-center gap-2 mb-6">
              {[1, 2, 3, 4, 5].map(n => (
                <button key={n} onClick={() => setEvaluation(n)}
                  className={`p-2 rounded-lg transition-colors ${evaluation >= n ? 'text-yellow-500' : 'text-gray-300 dark:text-gray-600'}`}>
                  <Star size={28} fill={evaluation >= n ? 'currentColor' : 'none'} />
                </button>
              ))}
            </div>
            <div className="flex gap-3 justify-end">
              <button onClick={() => setShowCloseDialog(false)} className="px-4 py-2 border border-gray-300 dark:border-gray-700 rounded-lg">Cancelar</button>
              <button onClick={handleClose} disabled={closing}
                className="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 disabled:opacity-50">
                {closing ? 'Fechando...' : 'Confirmar Fechamento'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

function DetailRow({ label, children }: { label: string; children: React.ReactNode }) {
  return (
    <div className="flex justify-between items-start">
      <dt className="text-gray-500 dark:text-gray-400">{label}</dt>
      <dd className="text-gray-900 dark:text-white text-right">{children}</dd>
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
