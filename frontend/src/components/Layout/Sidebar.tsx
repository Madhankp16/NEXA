import React from 'react';
import { NavLink } from 'react-router-dom';
import {
  LayoutDashboard,
  KanbanSquare,
  GitPullRequest,
  Building2,
  FolderLock,
  BarChart3,
  Layers,
  ShieldCheck
} from 'lucide-react';
import { useAppSelector } from '../../store';

export const Sidebar: React.FC = () => {
  const { user } = useAppSelector((state) => state.auth);

  const navItems = [
    { label: 'Dashboard', path: '/dashboard', icon: LayoutDashboard },
    { label: 'Projects & Tasks', path: '/kanban', icon: KanbanSquare },
    { label: 'Approvals & Workflows', path: '/approvals', icon: GitPullRequest },
    { label: 'Organization & Teams', path: '/organization', icon: Building2 },
    { label: 'Enterprise Documents', path: '/documents', icon: FolderLock },
    { label: 'Reports & Analytics', path: '/analytics', icon: BarChart3 },
  ];

  return (
    <aside className="w-64 border-r border-slate-800 bg-slate-900/95 flex flex-col justify-between shrink-0 h-screen sticky top-0">
      <div>
        {/* Brand Header */}
        <div className="h-16 px-6 flex items-center gap-3 border-b border-slate-800">
          <div className="w-9 h-9 rounded-xl bg-gradient-to-br from-indigo-500 via-indigo-600 to-violet-700 flex items-center justify-center shadow-lg shadow-indigo-500/25">
            <Layers className="w-5 h-5 text-white" />
          </div>
          <div>
            <h1 className="text-base font-bold tracking-tight text-white flex items-center gap-1.5">
              NexaWork <span className="text-[10px] bg-indigo-500/20 text-indigo-400 border border-indigo-500/30 px-1.5 py-0.5 rounded font-mono font-normal">v1.0</span>
            </h1>
            <p className="text-[11px] text-slate-400 font-medium">Enterprise Management</p>
          </div>
        </div>

        {/* Organization Badge */}
        <div className="px-4 py-3 mx-3 my-3 rounded-lg bg-slate-800/40 border border-slate-800/80 flex items-center gap-2.5">
          <Building2 className="w-4 h-4 text-indigo-400 shrink-0" />
          <div className="truncate">
            <p className="text-xs font-semibold text-slate-200 truncate">{user?.organizationName || 'Nexa Enterprise'}</p>
            <p className="text-[10px] text-slate-400">PostgreSQL Cloud Cluster</p>
          </div>
        </div>

        {/* Navigation Links */}
        <nav className="px-3 space-y-1">
          {navItems.map((item) => {
            const Icon = item.icon;
            return (
              <NavLink
                key={item.path}
                to={item.path}
                className={({ isActive }) =>
                  `flex items-center gap-3 px-3 py-2.5 rounded-lg text-xs font-medium transition-all ${
                    isActive
                      ? 'bg-indigo-600/15 text-indigo-400 border border-indigo-500/30 shadow-sm'
                      : 'text-slate-400 hover:text-slate-200 hover:bg-slate-800/60'
                  }`
                }
              >
                <Icon className="w-4 h-4 shrink-0" />
                <span>{item.label}</span>
              </NavLink>
            );
          })}
        </nav>
      </div>

      {/* Footer Security Pill */}
      <div className="p-4 border-t border-slate-800">
        <div className="flex items-center gap-2 px-3 py-2 rounded-lg bg-emerald-950/20 border border-emerald-500/20 text-emerald-400 text-[11px]">
          <ShieldCheck className="w-4 h-4 shrink-0 text-emerald-400" />
          <div>
            <p className="font-semibold">JWT + RBAC Active</p>
            <p className="text-[10px] text-slate-400">{user?.role} Tier</p>
          </div>
        </div>
      </div>
    </aside>
  );
};
