import React from 'react';
import { BarChart3, TrendingUp, ShieldCheck, CheckCircle2, Clock4, Database, Cpu } from 'lucide-react';

export const AnalyticsPage: React.FC = () => {
  return (
    <div className="space-y-6">
      {/* Header */}
      <div>
        <h2 className="text-xl font-bold tracking-tight text-white flex items-center gap-2">
          <BarChart3 className="w-5 h-5 text-indigo-400" />
          Enterprise Analytics & System Telemetry
        </h2>
        <p className="text-xs text-slate-400 mt-1">
          Historical project velocity, audit log streams, and database reporting metrics.
        </p>
      </div>

      {/* Architecture Highlights Banner */}
      <div className="p-6 rounded-2xl bg-gradient-to-r from-indigo-950/60 via-purple-950/40 to-slate-900 border border-indigo-500/30">
        <div className="flex items-center gap-3 mb-2">
          <Cpu className="w-5 h-5 text-indigo-400" />
          <h3 className="text-sm font-bold text-white">Clean Architecture & Full Stack Highlights</h3>
        </div>
        <p className="text-xs text-slate-300 leading-relaxed max-w-3xl">
          NexaWork combines <strong>Domain-Driven Design (DDD)</strong>, <strong>PostgreSQL with EF Core 8</strong>,
          <strong>Redis caching</strong>, <strong>JWT authentication with Refresh Token rotation</strong>, and
          <strong>SignalR real-time hubs</strong>. Built for real-world enterprise scalability and interview presentation.
        </p>

        <div className="grid grid-cols-2 sm:grid-cols-4 gap-4 mt-5">
          <div className="p-3 rounded-xl bg-slate-900/80 border border-slate-800">
            <span className="text-[10px] text-slate-400 font-mono">PRIMARY DB</span>
            <p className="text-xs font-bold text-indigo-300 mt-0.5">PostgreSQL 18</p>
          </div>
          <div className="p-3 rounded-xl bg-slate-900/80 border border-slate-800">
            <span className="text-[10px] text-slate-400 font-mono">BACKEND API</span>
            <p className="text-xs font-bold text-purple-300 mt-0.5">ASP.NET 8 Core</p>
          </div>
          <div className="p-3 rounded-xl bg-slate-900/80 border border-slate-800">
            <span className="text-[10px] text-slate-400 font-mono">REAL-TIME</span>
            <p className="text-xs font-bold text-emerald-300 mt-0.5">SignalR Hub</p>
          </div>
          <div className="p-3 rounded-xl bg-slate-900/80 border border-slate-800">
            <span className="text-[10px] text-slate-400 font-mono">CLIENT STATE</span>
            <p className="text-xs font-bold text-amber-300 mt-0.5">Redux Toolkit</p>
          </div>
        </div>
      </div>

      {/* KPI Matrix */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div className="p-5 rounded-2xl bg-slate-900 border border-slate-800">
          <h4 className="text-xs font-semibold text-slate-300 mb-3 flex items-center gap-2">
            <TrendingUp className="w-4 h-4 text-emerald-400" />
            Sprint Completion Velocity
          </h4>
          <p className="text-3xl font-extrabold text-emerald-400">92.4%</p>
          <p className="text-[11px] text-slate-400 mt-1">Average story-point clearance per sprint cycle</p>
        </div>

        <div className="p-5 rounded-2xl bg-slate-900 border border-slate-800">
          <h4 className="text-xs font-semibold text-slate-300 mb-3 flex items-center gap-2">
            <Clock4 className="w-4 h-4 text-amber-400" />
            Workflow Turnaround Time
          </h4>
          <p className="text-3xl font-extrabold text-amber-400">4.2 hrs</p>
          <p className="text-[11px] text-slate-400 mt-1">Average multi-tier manager approval latency</p>
        </div>

        <div className="p-5 rounded-2xl bg-slate-900 border border-slate-800">
          <h4 className="text-xs font-semibold text-slate-300 mb-3 flex items-center gap-2">
            <ShieldCheck className="w-4 h-4 text-indigo-400" />
            Security & Audit Conformance
          </h4>
          <p className="text-3xl font-extrabold text-indigo-400">100%</p>
          <p className="text-[11px] text-slate-400 mt-1">Every state transition tracked in AuditLogs table</p>
        </div>
      </div>
    </div>
  );
};
