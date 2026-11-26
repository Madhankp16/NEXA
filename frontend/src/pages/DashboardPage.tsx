import React, { useState, useEffect } from 'react';
import api from '../services/api';
import { DashboardKpi } from '../types';
import {
  Users,
  FolderGit2,
  CheckCircle2,
  Clock4,
  FileText,
  TrendingUp,
  Activity,
  ArrowUpRight,
  ShieldCheck,
  Building
} from 'lucide-react';
import { Link } from 'react-router-dom';

export const DashboardPage: React.FC = () => {
  const [data, setData] = useState<DashboardKpi | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchDashboard = async () => {
      try {
        const res = await api.get('/dashboard/summary');
        if (res.data?.success) {
          setData(res.data.data);
        }
      } catch (err) {
        console.error('Error fetching dashboard summary:', err);
      } finally {
        setLoading(false);
      }
    };

    fetchDashboard();
  }, []);

  if (loading) {
    return (
      <div className="space-y-6 animate-pulse">
        <div className="h-8 w-48 bg-slate-800 rounded-lg"></div>
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          {[1, 2, 3, 4].map((i) => (
            <div key={i} className="h-28 bg-slate-800/60 rounded-xl"></div>
          ))}
        </div>
      </div>
    );
  }

  const kpis = [
    {
      label: 'Total Workforce',
      value: data?.totalEmployees || 0,
      sub: 'Across 3 departments',
      icon: Users,
      color: 'from-blue-500/20 to-indigo-500/10 text-indigo-400 border-indigo-500/30',
    },
    {
      label: 'Active Projects',
      value: data?.totalProjects || 0,
      sub: 'All enterprise initiatives',
      icon: FolderGit2,
      color: 'from-purple-500/20 to-pink-500/10 text-purple-400 border-purple-500/30',
    },
    {
      label: 'Task Velocity',
      value: `${data?.completedTasks || 0} / ${(data?.activeTasks || 0) + (data?.completedTasks || 0)}`,
      sub: `${data?.projectCompletionRate || 0}% Completion Rate`,
      icon: CheckCircle2,
      color: 'from-emerald-500/20 to-teal-500/10 text-emerald-400 border-emerald-500/30',
    },
    {
      label: 'Pending Approvals',
      value: data?.pendingApprovals || 0,
      sub: 'Awaiting managerial sign-off',
      icon: Clock4,
      color: 'from-amber-500/20 to-orange-500/10 text-amber-400 border-amber-500/30',
    },
  ];

  return (
    <div className="space-y-6">
      {/* Title & Banner */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h2 className="text-xl font-bold tracking-tight text-white flex items-center gap-2">
            Enterprise Executive Dashboard
            <span className="inline-flex items-center gap-1 text-[11px] font-medium bg-emerald-500/15 text-emerald-400 border border-emerald-500/30 px-2 py-0.5 rounded-full">
              <span className="w-1.5 h-1.5 rounded-full bg-emerald-400 animate-pulse"></span>
              Live Synced
            </span>
          </h2>
          <p className="text-xs text-slate-400 mt-1">Real-time organizational performance, workflows, and task telemetry.</p>
        </div>

        <div className="flex items-center gap-2">
          <Link
            to="/kanban"
            className="px-3 py-2 rounded-lg bg-indigo-600 hover:bg-indigo-500 text-white text-xs font-semibold shadow-md transition-all flex items-center gap-1.5"
          >
            Open Kanban Board
            <ArrowUpRight className="w-3.5 h-3.5" />
          </Link>
        </div>
      </div>

      {/* KPI Cards */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        {kpis.map((kpi, idx) => {
          const Icon = kpi.icon;
          return (
            <div
              key={idx}
              className={`p-5 rounded-2xl bg-gradient-to-br ${kpi.color} bg-slate-900 border backdrop-blur-sm transition-all hover:translate-y-[-2px]`}
            >
              <div className="flex items-center justify-between mb-3">
                <span className="text-xs font-medium text-slate-300">{kpi.label}</span>
                <div className="p-2 rounded-xl bg-slate-800/70 border border-slate-700/60">
                  <Icon className="w-4 h-4" />
                </div>
              </div>
              <p className="text-2xl font-bold tracking-tight text-white">{kpi.value}</p>
              <p className="text-[11px] text-slate-400 mt-1">{kpi.sub}</p>
            </div>
          );
        })}
      </div>

      {/* Grid: Charts & Distributions */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Monthly Task Completion Velocity Chart */}
        <div className="lg:col-span-2 p-5 rounded-2xl bg-slate-900 border border-slate-800">
          <div className="flex items-center justify-between mb-4">
            <div>
              <h3 className="text-sm font-semibold text-white flex items-center gap-2">
                <TrendingUp className="w-4 h-4 text-indigo-400" />
                Task Delivery Velocity & Sprint Progress
              </h3>
              <p className="text-[11px] text-slate-400">Created vs completed tasks per month</p>
            </div>
            <div className="flex items-center gap-3 text-xs">
              <span className="flex items-center gap-1 text-slate-300">
                <span className="w-2.5 h-2.5 rounded-full bg-indigo-500"></span> Created
              </span>
              <span className="flex items-center gap-1 text-slate-300">
                <span className="w-2.5 h-2.5 rounded-full bg-emerald-400"></span> Completed
              </span>
            </div>
          </div>

          {/* SVG Bar Chart Visualization */}
          <div className="h-56 flex items-end justify-between gap-4 pt-6 px-2">
            {(data?.monthlyActivities || []).map((m, i) => {
              const maxVal = 80;
              const createdHeight = (m.created / maxVal) * 100;
              const completedHeight = (m.completed / maxVal) * 100;

              return (
                <div key={i} className="flex-1 flex flex-col items-center gap-2">
                  <div className="w-full flex items-end justify-center gap-1.5 h-40">
                    <div
                      style={{ height: `${createdHeight}%` }}
                      className="w-4 sm:w-5 bg-indigo-500/80 rounded-t-md hover:bg-indigo-400 transition-all"
                      title={`Created: ${m.created}`}
                    ></div>
                    <div
                      style={{ height: `${completedHeight}%` }}
                      className="w-4 sm:w-5 bg-emerald-500/80 rounded-t-md hover:bg-emerald-400 transition-all"
                      title={`Completed: ${m.completed}`}
                    ></div>
                  </div>
                  <span className="text-[11px] text-slate-400 font-medium">{m.month}</span>
                </div>
              );
            })}
          </div>
        </div>

        {/* Department Workforce Distribution */}
        <div className="p-5 rounded-2xl bg-slate-900 border border-slate-800">
          <h3 className="text-sm font-semibold text-white flex items-center gap-2 mb-1">
            <Building className="w-4 h-4 text-purple-400" />
            Department Allocation
          </h3>
          <p className="text-[11px] text-slate-400 mb-4">Staff headcount by core division</p>

          <div className="space-y-4">
            {(data?.departmentDistributions || []).map((dept, index) => {
              const total = data?.totalEmployees || 1;
              const pct = Math.round((dept.employeeCount / total) * 100);
              return (
                <div key={index} className="space-y-1.5">
                  <div className="flex items-center justify-between text-xs">
                    <span className="text-slate-300 font-medium">{dept.departmentName}</span>
                    <span className="text-slate-400 font-mono text-[11px]">{dept.employeeCount} engineers</span>
                  </div>
                  <div className="w-full h-2 rounded-full bg-slate-800 overflow-hidden">
                    <div
                      style={{ width: `${pct}%` }}
                      className={`h-full rounded-full ${
                        index === 0
                          ? 'bg-indigo-500'
                          : index === 1
                          ? 'bg-purple-500'
                          : 'bg-emerald-500'
                      }`}
                    ></div>
                  </div>
                </div>
              );
            })}
          </div>
        </div>
      </div>

      {/* Recent Approvals & Quick Actions */}
      <div className="p-5 rounded-2xl bg-slate-900 border border-slate-800">
        <div className="flex items-center justify-between mb-4">
          <h3 className="text-sm font-semibold text-white flex items-center gap-2">
            <Activity className="w-4 h-4 text-amber-400" />
            Recent Workflow Approvals
          </h3>
          <Link to="/approvals" className="text-xs text-indigo-400 hover:text-indigo-300 font-medium">
            View all workflows &rarr;
          </Link>
        </div>

        <div className="overflow-x-auto">
          <table className="w-full text-left text-xs">
            <thead>
              <tr className="border-b border-slate-800 text-slate-400">
                <th className="pb-3 font-medium">Request Title</th>
                <th className="pb-3 font-medium">Requester</th>
                <th className="pb-3 font-medium">Workflow Type</th>
                <th className="pb-3 font-medium">Status</th>
                <th className="pb-3 font-medium text-right">Action</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-800/60">
              {(data?.recentApprovals || []).map((app) => (
                <tr key={app.id} className="hover:bg-slate-800/30 transition-colors">
                  <td className="py-3 font-medium text-slate-200">{app.title}</td>
                  <td className="py-3 text-slate-400">{app.requesterName}</td>
                  <td className="py-3">
                    <span className="px-2 py-0.5 rounded-full bg-slate-800 text-[10px] text-slate-300 font-mono">
                      {app.type}
                    </span>
                  </td>
                  <td className="py-3">
                    <span
                      className={`px-2 py-0.5 rounded-full text-[10px] font-medium ${
                        app.status === 'Approved'
                          ? 'bg-emerald-500/10 text-emerald-400 border border-emerald-500/30'
                          : app.status === 'Rejected'
                          ? 'bg-rose-500/10 text-rose-400 border border-rose-500/30'
                          : 'bg-amber-500/10 text-amber-400 border border-amber-500/30'
                      }`}
                    >
                      {app.status}
                    </span>
                  </td>
                  <td className="py-3 text-right">
                    <Link
                      to="/approvals"
                      className="text-indigo-400 hover:text-indigo-300 font-medium"
                    >
                      Review
                    </Link>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
};
