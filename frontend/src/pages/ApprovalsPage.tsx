import React, { useState, useEffect } from 'react';
import api from '../services/api';
import { ApprovalRequest } from '../types';
import {
  GitPullRequest,
  CheckCircle,
  XCircle,
  Clock,
  Plus,
  Send,
  User,
  ShieldAlert,
  Calendar,
  Check
} from 'lucide-react';

export const ApprovalsPage: React.FC = () => {
  const [approvals, setApprovals] = useState<ApprovalRequest[]>([]);
  const [filterStatus, setFilterStatus] = useState<string>('all');
  const [loading, setLoading] = useState(true);

  // Process Modal
  const [selectedApproval, setSelectedApproval] = useState<ApprovalRequest | null>(null);
  const [comments, setComments] = useState('');
  const [processing, setProcessing] = useState(false);

  // New Request Modal
  const [showNewModal, setShowNewModal] = useState(false);
  const [reqTitle, setReqTitle] = useState('');
  const [reqDesc, setReqDesc] = useState('');
  const [reqType, setReqType] = useState(2); // BudgetApproval

  useEffect(() => {
    fetchApprovals();
  }, [filterStatus]);

  const fetchApprovals = async () => {
    try {
      const url = filterStatus === 'all' ? '/workflows/approvals' : `/workflows/approvals?status=${filterStatus}`;
      const res = await api.get(url);
      if (res.data?.success) {
        setApprovals(res.data.data);
      }
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleProcess = async (isApproved: boolean) => {
    if (!selectedApproval) return;
    setProcessing(true);

    try {
      const res = await api.post(
        `/workflows/approvals/${selectedApproval.id}/steps/${selectedApproval.currentStepNumber}/process`,
        {
          isApproved,
          comments,
        }
      );

      if (res.data?.success) {
        setSelectedApproval(null);
        setComments('');
        fetchApprovals();
      }
    } catch (err) {
      console.error(err);
    } finally {
      setProcessing(false);
    }
  };

  const handleCreateRequest = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!reqTitle) return;

    try {
      const res = await api.post('/workflows/approvals', {
        title: reqTitle,
        description: reqDesc,
        workflowType: reqType,
        payloadJson: JSON.stringify({ submittedBy: 'Staff', timestamp: new Date().toISOString() }),
      });

      if (res.data?.success) {
        setShowNewModal(false);
        setReqTitle('');
        setReqDesc('');
        fetchApprovals();
      }
    } catch (err) {
      console.error(err);
    }
  };

  const getWorkflowTypeName = (type: number) => {
    switch (type) {
      case 1:
        return 'Leave Request';
      case 2:
        return 'Budget Approval';
      case 3:
        return 'Expense Claim';
      case 4:
        return 'Document Sign-Off';
      default:
        return 'Initiative Approval';
    }
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h2 className="text-xl font-bold tracking-tight text-white flex items-center gap-2">
            <GitPullRequest className="w-5 h-5 text-indigo-400" />
            Workflow & Multi-Level Approvals
          </h2>
          <p className="text-xs text-slate-400 mt-1">
            Audit-trailed governance engine with manager and executive VP authorization steps.
          </p>
        </div>

        <button
          onClick={() => setShowNewModal(true)}
          className="px-3.5 py-2 rounded-xl bg-indigo-600 hover:bg-indigo-500 text-white text-xs font-semibold shadow-lg shadow-indigo-600/20 flex items-center gap-1.5 transition-all self-start sm:self-auto"
        >
          <Plus className="w-4 h-4" />
          Submit Approval Request
        </button>
      </div>

      {/* Filter Tabs */}
      <div className="flex items-center gap-2 border-b border-slate-800 pb-3">
        {['all', '1', '2', '3'].map((tab) => {
          const label = tab === 'all' ? 'All Requests' : tab === '1' ? 'Pending' : tab === '2' ? 'Approved' : 'Rejected';
          return (
            <button
              key={tab}
              onClick={() => setFilterStatus(tab)}
              className={`px-3 py-1.5 rounded-lg text-xs font-medium transition-all ${
                filterStatus === tab
                  ? 'bg-slate-800 text-indigo-400 border border-slate-700 shadow-sm'
                  : 'text-slate-400 hover:text-slate-200'
              }`}
            >
              {label}
            </button>
          );
        })}
      </div>

      {/* Approvals List */}
      <div className="space-y-4">
        {approvals.map((req) => (
          <div
            key={req.id}
            className="p-5 rounded-2xl bg-slate-900 border border-slate-800 hover:border-slate-700 transition-all shadow-md"
          >
            <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 pb-4 border-b border-slate-800/80">
              <div>
                <div className="flex items-center gap-2.5 mb-1.5">
                  <span className="px-2 py-0.5 rounded-full bg-indigo-500/15 text-indigo-400 border border-indigo-500/30 text-[10px] font-semibold">
                    {getWorkflowTypeName(req.workflowType)}
                  </span>
                  <span
                    className={`px-2 py-0.5 rounded-full text-[10px] font-semibold ${
                      req.status === 2
                        ? 'bg-emerald-500/15 text-emerald-400 border border-emerald-500/30'
                        : req.status === 3
                        ? 'bg-rose-500/15 text-rose-400 border border-rose-500/30'
                        : 'bg-amber-500/15 text-amber-400 border border-amber-500/30'
                    }`}
                  >
                    {req.status === 2 ? 'Approved' : req.status === 3 ? 'Rejected' : 'Pending Action'}
                  </span>
                </div>
                <h4 className="text-sm font-bold text-white">{req.title}</h4>
                <p className="text-xs text-slate-400 mt-1">{req.description}</p>
              </div>

              <div className="flex items-center gap-3">
                {req.status === 1 && (
                  <button
                    onClick={() => setSelectedApproval(req)}
                    className="px-3 py-1.5 rounded-lg bg-indigo-600 hover:bg-indigo-500 text-white text-xs font-medium shadow-md transition-all flex items-center gap-1.5"
                  >
                    Review & Decide
                  </button>
                )}
              </div>
            </div>

            {/* Multi-step Visualizer */}
            <div className="pt-4 flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
              <div className="flex items-center gap-2 text-xs text-slate-400">
                <User className="w-3.5 h-3.5 text-slate-400" />
                <span>Requester: <strong className="text-slate-200">{req.requesterName}</strong></span>
              </div>

              {/* Steps Progress */}
              <div className="flex items-center gap-2 w-full sm:w-auto">
                {req.steps.map((st, i) => (
                  <div key={st.id} className="flex items-center gap-2">
                    <div
                      className={`flex items-center gap-1.5 px-2.5 py-1 rounded-lg text-[11px] font-medium border ${
                        st.status === 2
                          ? 'bg-emerald-950/40 border-emerald-500/40 text-emerald-300'
                          : st.status === 3
                          ? 'bg-rose-950/40 border-rose-500/40 text-rose-300'
                          : 'bg-slate-800/80 border-slate-700 text-slate-300'
                      }`}
                    >
                      {st.status === 2 ? (
                        <Check className="w-3 h-3 text-emerald-400" />
                      ) : (
                        <span className="w-3.5 h-3.5 rounded-full bg-slate-700 text-[9px] flex items-center justify-center font-bold">
                          {st.stepNumber}
                        </span>
                      )}
                      <span>{st.stepName}</span>
                    </div>
                    {i < req.steps.length - 1 && <span className="text-slate-600">&rarr;</span>}
                  </div>
                ))}
              </div>
            </div>
          </div>
        ))}

        {approvals.length === 0 && (
          <div className="p-8 text-center rounded-2xl bg-slate-900 border border-slate-800 text-slate-400 text-xs">
            No approval requests found under current filter.
          </div>
        )}
      </div>

      {/* Decision Modal */}
      {selectedApproval && (
        <div className="fixed inset-0 bg-black/60 backdrop-blur-sm z-50 flex items-center justify-center p-4">
          <div className="bg-slate-900 border border-slate-800 rounded-2xl w-full max-w-lg p-6 shadow-2xl">
            <h3 className="text-base font-bold text-white mb-2">Process Approval Request</h3>
            <p className="text-xs text-slate-400 mb-4">{selectedApproval.title}</p>

            <div className="space-y-4">
              <div>
                <label className="block text-xs font-medium text-slate-300 mb-1">
                  Manager Review Comments
                </label>
                <textarea
                  rows={3}
                  value={comments}
                  onChange={(e) => setComments(e.target.value)}
                  placeholder="State justification or audit notes for this decision..."
                  className="w-full bg-slate-800 border border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-100 focus:outline-none focus:ring-2 focus:ring-indigo-500"
                />
              </div>

              <div className="flex justify-end gap-3 pt-3 border-t border-slate-800">
                <button
                  type="button"
                  onClick={() => setSelectedApproval(null)}
                  className="px-4 py-2 rounded-xl bg-slate-800 hover:bg-slate-700 text-xs font-medium text-slate-300"
                >
                  Cancel
                </button>
                <button
                  type="button"
                  disabled={processing}
                  onClick={() => handleProcess(false)}
                  className="px-4 py-2 rounded-xl bg-rose-600 hover:bg-rose-500 text-xs font-semibold text-white flex items-center gap-1.5"
                >
                  <XCircle className="w-3.5 h-3.5" />
                  Reject Request
                </button>
                <button
                  type="button"
                  disabled={processing}
                  onClick={() => handleProcess(true)}
                  className="px-4 py-2 rounded-xl bg-emerald-600 hover:bg-emerald-500 text-xs font-semibold text-white flex items-center gap-1.5"
                >
                  <CheckCircle className="w-3.5 h-3.5" />
                  Approve Step
                </button>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* New Request Modal */}
      {showNewModal && (
        <div className="fixed inset-0 bg-black/60 backdrop-blur-sm z-50 flex items-center justify-center p-4">
          <div className="bg-slate-900 border border-slate-800 rounded-2xl w-full max-w-lg p-6 shadow-2xl">
            <h3 className="text-base font-bold text-white mb-4">Submit Workflow Approval</h3>
            <form onSubmit={handleCreateRequest} className="space-y-4">
              <div>
                <label className="block text-xs font-medium text-slate-300 mb-1">Approval Title</label>
                <input
                  type="text"
                  required
                  value={reqTitle}
                  onChange={(e) => setReqTitle(e.target.value)}
                  placeholder="e.g. Q4 Cloud Database Tier Upgrade"
                  className="w-full bg-slate-800 border border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-100 focus:outline-none focus:ring-2 focus:ring-indigo-500"
                />
              </div>

              <div>
                <label className="block text-xs font-medium text-slate-300 mb-1">Workflow Category</label>
                <select
                  value={reqType}
                  onChange={(e) => setReqType(Number(e.target.value))}
                  className="w-full bg-slate-800 border border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-100 focus:outline-none focus:ring-2 focus:ring-indigo-500"
                >
                  <option value={1}>Leave Request</option>
                  <option value={2}>Budget Approval</option>
                  <option value={3}>Expense Claim</option>
                  <option value={4}>Document Sign-Off</option>
                </select>
              </div>

              <div>
                <label className="block text-xs font-medium text-slate-300 mb-1">Detailed Description & Justification</label>
                <textarea
                  rows={3}
                  value={reqDesc}
                  onChange={(e) => setReqDesc(e.target.value)}
                  placeholder="Provide scope, dollar impact, and justification..."
                  className="w-full bg-slate-800 border border-slate-700 rounded-xl px-3 py-2 text-xs text-slate-100 focus:outline-none focus:ring-2 focus:ring-indigo-500"
                />
              </div>

              <div className="flex justify-end gap-3 pt-3 border-t border-slate-800">
                <button
                  type="button"
                  onClick={() => setShowNewModal(false)}
                  className="px-4 py-2 rounded-xl bg-slate-800 hover:bg-slate-700 text-xs font-medium text-slate-300"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="px-4 py-2 rounded-xl bg-indigo-600 hover:bg-indigo-500 text-xs font-semibold text-white"
                >
                  Submit For Review
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};
