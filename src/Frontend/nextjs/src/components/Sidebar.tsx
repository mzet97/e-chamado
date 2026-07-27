'use client';

import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { useAuth } from '@/contexts/AuthContext';
import {
  LayoutDashboard, Ticket, Plus, Brain, Filter, Settings,
  FolderTree, Tag, Building2, Layers, List, Users, Shield,
  ChevronDown, ChevronRight, X
} from 'lucide-react';
import { useState } from 'react';

interface NavItem {
  label: string;
  href?: string;
  icon: React.ReactNode;
  children?: NavItem[];
  roles?: string[];
}

const navItems: NavItem[] = [
  { label: 'Dashboard', href: '/', icon: <LayoutDashboard size={20} /> },
  {
    label: 'Chamados',
    icon: <Ticket size={20} />,
    children: [
      { label: 'Lista de Chamados', href: '/orders', icon: <List size={18} /> },
      { label: 'Novo Chamado', href: '/orders/create', icon: <Plus size={18} /> },
      {
        label: 'Busca Avançada',
        icon: <Brain size={18} />,
        children: [
          { label: 'Com IA (Linguagem Natural)', href: '/orders/ai', icon: <Brain size={16} /> },
          { label: 'Com OData', href: '/orders/odata', icon: <Filter size={16} /> },
        ],
      },
    ],
  },
  {
    label: 'Administração',
    icon: <Settings size={20} />,
    roles: ['Admin'],
    children: [
      { label: 'Categorias', href: '/admin/categories', icon: <FolderTree size={18} /> },
      { label: 'Subcategorias', href: '/admin/subcategories', icon: <Tag size={18} /> },
      { label: 'Departamentos', href: '/admin/departments', icon: <Building2 size={18} /> },
      { label: 'Tipos de Chamado', href: '/admin/ordertypes', icon: <Layers size={18} /> },
      { label: 'Status', href: '/admin/statustypes', icon: <List size={18} /> },
      { label: 'Usuários', href: '/admin/users', icon: <Users size={18} />, roles: ['Admin'] },
      { label: 'Perfis', href: '/admin/roles', icon: <Shield size={18} />, roles: ['Admin'] },
    ],
  },
];

function NavItemComponent({ item, depth = 0 }: { item: NavItem; depth?: number }) {
  const pathname = usePathname();
  const { hasRole } = useAuth();
  const [expanded, setExpanded] = useState(false);

  // Role check
  if (item.roles && !item.roles.some(r => hasRole(r))) return null;

  const isActive = item.href && pathname === item.href;
  const hasChildren = item.children && item.children.length > 0;

  if (hasChildren) {
    return (
      <div>
        <button
          onClick={() => setExpanded(!expanded)}
          className={`w-full flex items-center justify-between px-3 py-2 rounded-lg text-sm
            ${depth > 0 ? 'pl-6' : ''}
            hover:bg-gray-100 dark:hover:bg-gray-800 text-gray-700 dark:text-gray-300`}
        >
          <span className="flex items-center gap-3">
            {item.icon}
            {item.label}
          </span>
          {expanded ? <ChevronDown size={16} /> : <ChevronRight size={16} />}
        </button>
        {expanded && (
          <div className="ml-2 border-l border-gray-200 dark:border-gray-700">
            {item.children!.map((child, i) => (
              <NavItemComponent key={i} item={child} depth={depth + 1} />
            ))}
          </div>
        )}
      </div>
    );
  }

  return (
    <Link
      href={item.href!}
      className={`flex items-center gap-3 px-3 py-2 rounded-lg text-sm transition-colors
        ${depth > 0 ? 'pl-6' : ''}
        ${isActive
          ? 'bg-blue-50 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400 font-medium'
          : 'hover:bg-gray-100 dark:hover:bg-gray-800 text-gray-700 dark:text-gray-300'
        }`}
    >
      {item.icon}
      {item.label}
    </Link>
  );
}

interface SidebarProps {
  open: boolean;
  onClose: () => void;
}

export default function Sidebar({ open, onClose }: SidebarProps) {
  return (
    <>
      {/* Mobile overlay */}
      {open && (
        <div className="fixed inset-0 z-40 bg-black/50 lg:hidden" onClick={onClose} />
      )}

      {/* Sidebar */}
      <aside className={`fixed top-0 left-0 z-50 h-full w-64 bg-white dark:bg-gray-900 border-r border-gray-200 dark:border-gray-800 transform transition-transform lg:translate-x-0 ${open ? 'translate-x-0' : '-translate-x-full'}`}>
        <div className="flex items-center justify-between p-4 border-b border-gray-200 dark:border-gray-800">
          <div className="flex items-center gap-2">
            <div className="w-8 h-8 bg-blue-600 rounded-lg flex items-center justify-center">
              <Ticket size={18} className="text-white" />
            </div>
            <span className="text-lg font-bold text-gray-900 dark:text-white">EChamado</span>
          </div>
          <button onClick={onClose} className="lg:hidden p-1 rounded hover:bg-gray-100 dark:hover:bg-gray-800">
            <X size={20} />
          </button>
        </div>

        <nav className="p-3 space-y-1 overflow-y-auto h-[calc(100%-64px)]">
          {navItems.map((item, i) => (
            <NavItemComponent key={i} item={item} />
          ))}
        </nav>
      </aside>
    </>
  );
}
