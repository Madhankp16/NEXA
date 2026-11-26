import React, { useState, useEffect } from 'react';
import api from '../services/api';
import { Project, TaskItem } from '../types';
import {
  Plus,
  Clock,
  MessageSquare,
  Paperclip,
  CheckCircle2,
  AlertCircle,
  Filter,
  User,
  MoreVertical,
  ArrowRight,
  FolderPlus
} from 'lucide-react';

const COLUMNS = [
  { id: 1, name: 'To Do', color: 'border-slate-700 bg-slate-800/30' },
  { id: 2, name: 'In Progress', color: 'border-indigo-500/40 bg-indigo-950/20' },
  { id: 3, name: 'In Review', color: 'border-amber-500/40 bg-amber-950/20' },
  { id: 4, name: 'Done', color: 'border-emerald-500/40 bg-emerald-950/20' },
];

export const KanbanPage: React.FC = () => {
  const [projects, setProjects] = useState<Project[]>([]);
  const [selectedProjectId, setSelectedProjectId] = useState<string>('');
  const [tasks, setTasks] = useState<TaskItem[]>([]);
  const [loading, setLoading] = useState(true);

  // New Task Modal
  const [showTaskModal, setShowTaskModal] = useState(false);
  const [newTitle, setNewTitle] = useState('');
  const [newDesc, setNewDesc] = useState('');
  const [newPriority, setNewPriority] = useState(2);
  const [newHours, setNewHours] = useState(8);

  // New Project Modal
  const [showProjectModal, setShowProjectModal] = useState(false);
  const [projectName, setProjectName] = useState('');
  const [projectKey, setProjectKey] = useState('');
  const [projectDesc, setProjectDesc] = useState('');

  useEffect(() => {
    fetchProjects();
  }, []);

  useEffect(() => {
    if (selectedProjectId) {
      fetchTasks(selectedProjectId);
    }
  }, [selectedProjectId]);

  const fetchProjects = async () => {
    try {
      const res = await api.get('/projects');
      if (res.data?.success && res.data.data?.length > 0) {
        setProjects(res.data.data);
        setSelectedProjectId(res.data.data[0].id);
      }
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const fetchTasks = async (projId: string) => {
    try {
      const res = await api.get(`/tasks?projectId=${projId}`);
      if (res.data?.success) {
        setTasks(res.data.data);
      }
    } catch (err) {
      console.error(err);
    }
  };

  const handleCreateTask = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedProjectId || !newTitle) return;

    try {
      const res = await api.post('/tasks', {
        projectId: selectedProjectId,
        title: newTitle,
        description: newDesc,
        priority: newPriority,
        estimatedHours: newHours,
      });

      if (res.data?.success) {
        setTasks((prev) => [res.data.data, ...prev]);
        setShowTaskModal(false);
        setNewTitle('');
        setNewDesc('');
      }
    } catch (err) {
      console.error(err);
    }
  };

  const handleCreateProject = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!projectName || !projectKey) return;

    try {
      const res = await api.post('/projects', {
        name: projectName,
        key: projectKey.toUpperCase(),
        description: projectDesc,
      });

      if (res.data?.success) {
        setProjects((prev) => [res.data.data, ...prev]);
        setSelectedProjectId(res.data.data.id);
        setShowProjectModal(false);
        setProjectName('');
        setProjectKey('');
        setProjectDesc('');
      }
    } catch (err) {
      console.error(err);
    }
  };

  const moveTaskStatus = async (taskId: string, currentStatus: number, nextStatus: number) => {
    try {
      await api.put(`/tasks/${taskId}/status`, {
        status: nextStatus,
        orderIndex: 0,
      });

      setTasks((prev) =>
        prev.map((t) => (t.id === taskId ? { ...t, status: nextStatus } : t))
      );
    } catch (err) {
      console.error(err);
    }
  };

  const getPriorityBadge = (priority: number) => {
    switch (priority) {
      case 4:
        return <span className="px-1.5 py-0.5 rounded text-[10px] font-semibold bg-rose-500/20 text-rose-400 border border-rose-500/30">Urgent</span>;
      case 3:
        return <span className="px-1.5 py-0.5 rounded text-[10px] font-semibold bg-amber-500/20 text-amber-400 border border-amber-500/30">High</span>;
      case 2:
        return <span className="px-1.5 py-0.5 rounded text-[10px] font-semibold bg-blue-500/20 text-blue-400 border border-blue-500/30">Medium</span>;
      default:
        return <span className="px-1.5 py-0.5 rounded text-[10px] font-semibold bg-slate-500/20 text-slate-400 border border-slate-500/30">Low</span>;
    }
  };

  const currentProject = projects.find((p) => p.id === selectedProjectId);

  return (
    <div className="space-y-6">
      {/* Top Header & Project Select */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div className="flex items-center gap-3">
          <select
            value={selectedProjectId}
            onChange={(e) => setSelectedProjectId(e.target.value)}
            className="bg-slate-900 border border-slate-700 rounded-xl px-3.5 py-2 text-sm text-slate-100 font-semibold focus:outline-none focus:ring-2 focus:ring-indigo-500"
          >
            {projects.map((p) => (
              <option key={p.id} value={p.id}>
                [{p.key}] {p.name}
              </option>
            ))}
          </select>

          <button
            onClick={() => setShowProjectModal(true)}
            className="p-2 rounded-xl bg-slate-800 hover:bg-slate-700 text-slate-300 transition-colors"
            title="Create Project"
          >
            <FolderPlus className="w-4 h-4" />
          </button>
        </div>

        <div className="flex items-center gap-3">
          <button
            onClick={() => setShowTaskModal(true)}
            className="px-3.5 py-2 rounded-xl bg-indigo-600 hover:bg-indigo-500 text-white text-xs font-semibold shadow-lg shadow-indigo-600/20 flex items-center gap-1.5 transition-all"
          >
            <Plus className="w-4 h-4" />
            Add Task
          </button>
        </div>
      </div>

      {/* Project Meta Bar */}
      {currentProject && (
        <div className="p-4 rounded-xl bg-slate-900/60 border border-slate-800 flex flex-wrap items-center justify-between gap-4 text-xs">
          <div>
            <p className="text-slate-400">{currentProject.description || 'Enterprise project workspace'}</p>
          </div>
          <div className="flex items-center gap-4 text-slate-400">
            <span>Owner: <strong className="text-slate-200">{currentProject.ownerName}</strong></span>
            <span>Tasks: <strong className="text-slate-200">{tasks.length}</strong></span>
            <span>Progress: <strong className="text-emerald-400">{currentProject.progressPercentage}%</strong></span>
          </div>
        </div>
      )}

      {/* Kanban Board Columns */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        {COLUMNS.map((col) => {
          const colTasks = tasks.filter((t) => t.status === col.id);
          return (
            <div
              key={col.id}
              className={`rounded-2xl p-4 border ${col.color} flex flex-col h-[calc(100vh-250px)] min-h-[450px]`}
            >
              {/* Column Header */}
              <div className="flex items-center justify-between pb-3 border-b border-slate-800/80 mb-3">
                <div className="flex items-center gap-2">
                  <h4 className="text-xs font-bold uppercase tracking-wider text-slate-200">{col.name}</h4>
                  <span className="w-5 h-5 rounded-full bg-slate-800 text-[11px] font-bold text-slate-400 flex items-center justify-center">
                    {colTasks.length}
                  </span>
                </div>
              </div>

              {/* Task Items */}
              <div className="flex-1 overflow-y-auto space-y-3 pr-1">
                {colTasks.map((task) => (
                  <div
                    key={task.id}
                    className="p-4 rounded-xl bg-slate-900/90 border border-slate-800 hover:border-slate-700 shadow-md transition-all group"
                  >
                    <div className="flex items-center justify-between mb-2">
                      <span className="font-mono text-[10px] text-indigo-400 font-semibold">
                        {task.projectKey}-{task.orderIndex + 1}
                      </span>
                      {getPriorityBadge(task.priority)}
                    </div>

                    <h5 className="text-xs font-semibold text-slate-100 leading-snug mb-1">
                      {task.title}
                    </h5>

                    {task.description && (
                      <p className="text-[11px] text-slate-400 line-clamp-2 mb-3">
                        {task.description}
                      </p>
                    )}

                    {/* Footer Controls & Assignee */}
                    <div className="pt-2 border-t border-slate-800/60 flex items-center justify-between text-[11px]">
                      <div className="flex items-center gap-1.5 text-slate-400">
                        <User className="w-3.5 h-3.5 text-slate-400" />
                        <span className="truncate max-w-[80px]">{task.assigneeName || 'Unassigned'}</span>
                      </div>

                      {/* Fast Column Status Mover */}
                      <div className="flex items-center gap-1">
                        {col.id > 1 && (
                          <button
                            onClick={() => moveTaskStatus(task.id, col.id, col.id - 1)}
                            title="Move back"
                            className="p-1 rounded hover:bg-slate-800 text-slate-400 hover:text-slate-200 text-[10px]"
                          >
                            &larr;
                          </button>
                        )}
                        {col.id < 4 && (
                          <button
                            onClick={() => moveTaskStatus(task.id, col.id, col.id + 1)}
                            title="Advance forward"
                            className="p-1 rounded hover:bg-slate-800 text-indigo-400 hover:text-indigo-300 text-[10px] font-bold"
                          >
                            &rarr;
                          </button>
                        )}
                      </div>
                    </div>
                  </div>
                ))}

                {colTasks.length === 0 && (
                  <div className="h-28 border border-dashed border-slate-800 rounded-xl flex items-center justify-center text-xs text-slate-500">
                    No tasks in {col.name}
                  </div>
                )}
              </div>
            </div>
          );
        })}
      </div>

      {/* Create Task Modal */}
      {showTaskModal && (
        <div className="fixed inset-0 bg-black/60 backdrop-blur-sm z-50 flex items-center justify-center p-4">
          <div className="bg-slate-900 border border-slate-800 rounded-2xl w-full max-w-lg p-6 shadow-2xl">
            <h3 className="text-base font-bold text-white mb-4">Create Enterprise Task</h3>
            <form onSubmit={handleCreateTask} className="space-y-4">
              <div>
                <label className="block text-xs font-medium text-slate-300 mb-1">Task Title</label>
                <input
                  type="text"
                  required
                  value={newTitle}
                  onChange={(e) => setNewTitle(e.target.value)}
                  placeholder="e.g. Implement OAuth Google SSO Provider"
                  className="w-full bg-slate-800 border border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-100 focus:outline-none focus:ring-2 focus:ring-indigo-500"
                />
              </div>

              <div>
                <label className="block text-xs font-medium text-slate-300 mb-1">Description</label>
                <textarea
                  rows={3}
                  value={newDesc}
                  onChange={(e) => setNewDesc(e.target.value)}
                  placeholder="Technical acceptance criteria and workflow context..."
                  className="w-full bg-slate-800 border border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-100 focus:outline-none focus:ring-2 focus:ring-indigo-500"
                />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-xs font-medium text-slate-300 mb-1">Priority</label>
                  <select
                    value={newPriority}
                    onChange={(e) => setNewPriority(Number(e.target.value))}
                    className="w-full bg-slate-800 border border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-100 focus:outline-none focus:ring-2 focus:ring-indigo-500"
                  >
                    <option value={1}>Low</option>
                    <option value={2}>Medium</option>
                    <option value={3}>High</option>
                    <option value={4}>Urgent</option>
                  </select>
                </div>
                <div>
                  <label className="block text-xs font-medium text-slate-300 mb-1">Estimated Hours</label>
                  <input
                    type="number"
                    value={newHours}
                    onChange={(e) => setNewHours(Number(e.target.value))}
                    className="w-full bg-slate-800 border border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-100 focus:outline-none focus:ring-2 focus:ring-indigo-500"
                  />
                </div>
              </div>

              <div className="flex justify-end gap-3 pt-3 border-t border-slate-800">
                <button
                  type="button"
                  onClick={() => setShowTaskModal(false)}
                  className="px-4 py-2 rounded-xl bg-slate-800 hover:bg-slate-700 text-xs font-medium text-slate-300"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="px-4 py-2 rounded-xl bg-indigo-600 hover:bg-indigo-500 text-xs font-semibold text-white"
                >
                  Add Task
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Create Project Modal */}
      {showProjectModal && (
        <div className="fixed inset-0 bg-black/60 backdrop-blur-sm z-50 flex items-center justify-center p-4">
          <div className="bg-slate-900 border border-slate-800 rounded-2xl w-full max-w-lg p-6 shadow-2xl">
            <h3 className="text-base font-bold text-white mb-4">Create New Project</h3>
            <form onSubmit={handleCreateProject} className="space-y-4">
              <div>
                <label className="block text-xs font-medium text-slate-300 mb-1">Project Name</label>
                <input
                  type="text"
                  required
                  value={projectName}
                  onChange={(e) => setProjectName(e.target.value)}
                  placeholder="e.g. AI Workflow Optimization"
                  className="w-full bg-slate-800 border border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-100 focus:outline-none focus:ring-2 focus:ring-indigo-500"
                />
              </div>

              <div>
                <label className="block text-xs font-medium text-slate-300 mb-1">Key (Uppercase, 2-6 chars)</label>
                <input
                  type="text"
                  required
                  maxLength={6}
                  value={projectKey}
                  onChange={(e) => setProjectKey(e.target.value.toUpperCase())}
                  placeholder="e.g. AIW"
                  className="w-full bg-slate-800 border border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-100 font-mono uppercase focus:outline-none focus:ring-2 focus:ring-indigo-500"
                />
              </div>

              <div>
                <label className="block text-xs font-medium text-slate-300 mb-1">Description</label>
                <textarea
                  rows={2}
                  value={projectDesc}
                  onChange={(e) => setProjectDesc(e.target.value)}
                  className="w-full bg-slate-800 border border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-100 focus:outline-none focus:ring-2 focus:ring-indigo-500"
                />
              </div>

              <div className="flex justify-end gap-3 pt-3 border-t border-slate-800">
                <button
                  type="button"
                  onClick={() => setShowProjectModal(false)}
                  className="px-4 py-2 rounded-xl bg-slate-800 hover:bg-slate-700 text-xs font-medium text-slate-300"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="px-4 py-2 rounded-xl bg-indigo-600 hover:bg-indigo-500 text-xs font-semibold text-white"
                >
                  Create Project
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};
