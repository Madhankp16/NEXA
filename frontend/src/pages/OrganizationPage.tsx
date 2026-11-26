import React, { useState, useEffect } from 'react';
import api from '../services/api';
import { Building2, Users, Shield, Layers, Mail, Briefcase } from 'lucide-react';
import { UserProfile } from '../types';

interface Department {
  id: string;
  name: string;
  code: string;
  description?: string;
  managerName?: string;
  teamCount: number;
  memberCount: number;
}

interface Team {
  id: string;
  departmentName: string;
  name: string;
  description?: string;
  teamLeadName?: string;
  memberCount: number;
}

export const OrganizationPage: React.FC = () => {
  const [departments, setDepartments] = useState<Department[]>([]);
  const [teams, setTeams] = useState<Team[]>([]);
  const [members, setMembers] = useState<UserProfile[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [depRes, teamRes, memRes] = await Promise.all([
          api.get('/organizations/departments'),
          api.get('/organizations/teams'),
          api.get('/organizations/members'),
        ]);

        if (depRes.data?.success) setDepartments(depRes.data.data);
        if (teamRes.data?.success) setTeams(teamRes.data.data);
        if (memRes.data?.success) setMembers(memRes.data.data);
      } catch (err) {
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  return (
    <div className="space-y-6">
      {/* Header */}
      <div>
        <h2 className="text-xl font-bold tracking-tight text-white flex items-center gap-2">
          <Building2 className="w-5 h-5 text-indigo-400" />
          Enterprise Organization & Workforce
        </h2>
        <p className="text-xs text-slate-400 mt-1">
          Corporate department hierarchies, agile squads, and role-based access management.
        </p>
      </div>

      {/* Departments Grid */}
      <div>
        <h3 className="text-sm font-semibold text-slate-200 mb-3 flex items-center gap-2">
          <Layers className="w-4 h-4 text-purple-400" />
          Operating Divisions & Departments
        </h3>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          {departments.map((dept) => (
            <div
              key={dept.id}
              className="p-5 rounded-2xl bg-slate-900 border border-slate-800 hover:border-slate-700 transition-all shadow-md"
            >
              <div className="flex items-center justify-between mb-2">
                <span className="font-mono text-xs font-bold text-indigo-400 bg-indigo-500/10 px-2 py-0.5 rounded border border-indigo-500/20">
                  {dept.code}
                </span>
                <span className="text-xs text-slate-400">{dept.memberCount} staff members</span>
              </div>
              <h4 className="text-sm font-bold text-white mb-1">{dept.name}</h4>
              <p className="text-xs text-slate-400 line-clamp-2 mb-4">{dept.description}</p>

              <div className="pt-3 border-t border-slate-800/80 flex items-center justify-between text-xs text-slate-400">
                <span>Manager: <strong className="text-slate-200">{dept.managerName || 'Sarah Connor'}</strong></span>
                <span>{dept.teamCount} Teams</span>
              </div>
            </div>
          ))}
        </div>
      </div>

      {/* Teams Grid */}
      <div>
        <h3 className="text-sm font-semibold text-slate-200 mb-3 flex items-center gap-2">
          <Users className="w-4 h-4 text-emerald-400" />
          Agile Teams & Squads
        </h3>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          {teams.map((t) => (
            <div
              key={t.id}
              className="p-4 rounded-xl bg-slate-900/60 border border-slate-800 hover:border-slate-700 transition-all flex items-center justify-between"
            >
              <div>
                <span className="text-[10px] text-slate-400 font-semibold uppercase">{t.departmentName}</span>
                <h4 className="text-xs font-bold text-slate-100">{t.name}</h4>
                <p className="text-[11px] text-slate-400 mt-0.5">{t.description}</p>
              </div>
              <div className="text-right shrink-0">
                <span className="text-xs font-mono font-semibold text-emerald-400 bg-emerald-500/10 px-2 py-0.5 rounded border border-emerald-500/20">
                  {t.memberCount} engineers
                </span>
                <p className="text-[10px] text-slate-400 mt-1">Lead: {t.teamLeadName || 'Alex Vance'}</p>
              </div>
            </div>
          ))}
        </div>
      </div>

      {/* Workforce Directory Table */}
      <div className="p-5 rounded-2xl bg-slate-900 border border-slate-800">
        <h3 className="text-sm font-semibold text-white mb-4 flex items-center gap-2">
          <Briefcase className="w-4 h-4 text-sky-400" />
          Enterprise Staff Directory
        </h3>

        <div className="overflow-x-auto">
          <table className="w-full text-left text-xs">
            <thead>
              <tr className="border-b border-slate-800 text-slate-400 font-medium">
                <th className="pb-3">Name</th>
                <th className="pb-3">Corporate Email</th>
                <th className="pb-3">Designation</th>
                <th className="pb-3">Security Role</th>
                <th className="pb-3 text-right">Status</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-800/60">
              {members.map((u) => (
                <tr key={u.id} className="hover:bg-slate-800/30 transition-colors">
                  <td className="py-3 font-semibold text-slate-100 flex items-center gap-2.5">
                    <div className="w-7 h-7 rounded-full bg-gradient-to-tr from-indigo-500 to-purple-600 flex items-center justify-center font-bold text-[10px] text-white">
                      {u.firstName?.[0]}{u.lastName?.[0]}
                    </div>
                    <span>{u.fullName || `${u.firstName} ${u.lastName}`}</span>
                  </td>
                  <td className="py-3 text-slate-400 font-mono text-[11px]">{u.email}</td>
                  <td className="py-3 text-slate-300">{u.jobTitle || 'Contributor'}</td>
                  <td className="py-3">
                    <span className="px-2 py-0.5 rounded-full text-[10px] font-semibold bg-indigo-500/15 text-indigo-400 border border-indigo-500/30">
                      {u.role}
                    </span>
                  </td>
                  <td className="py-3 text-right">
                    <span className="px-2 py-0.5 rounded-full text-[10px] font-semibold bg-emerald-500/10 text-emerald-400 border border-emerald-500/30">
                      Active
                    </span>
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
