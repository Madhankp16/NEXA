export interface UserProfile {
  id: string;
  organizationId: string;
  organizationName: string;
  email: string;
  firstName: string;
  lastName: string;
  fullName: string;
  jobTitle?: string;
  avatarUrl?: string;
  role: string;
  permissions: string[];
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  user: UserProfile;
}

export interface Project {
  id: string;
  organizationId: string;
  name: string;
  key: string;
  description?: string;
  status: number; // 1: Planning, 2: Active, 3: OnHold, 4: Completed, 5: Archived
  priority: number; // 1: Low, 2: Medium, 3: High, 4: Urgent
  startDate?: string;
  endDate?: string;
  budget?: number;
  ownerName: string;
  taskCount: number;
  completedTaskCount: number;
  memberCount: number;
  progressPercentage: number;
}

export interface TaskItem {
  id: string;
  projectId: string;
  projectName: string;
  projectKey: string;
  title: string;
  description?: string;
  status: number; // 1: Todo, 2: InProgress, 3: InReview, 4: Done
  priority: number; // 1: Low, 2: Medium, 3: High, 4: Urgent
  orderIndex: number;
  assigneeId?: string;
  assigneeName?: string;
  assigneeAvatar?: string;
  reporterName: string;
  dueDate?: string;
  estimatedHours: number;
  loggedHours: number;
  commentCount: number;
  attachmentCount: number;
  createdAt: string;
}

export interface ApprovalStep {
  id: string;
  stepNumber: number;
  stepName: string;
  approverUserName?: string;
  approverRole?: string;
  status: number; // 1: Pending, 2: Approved, 3: Rejected, 4: Skipped
  comments?: string;
  actionDate?: string;
}

export interface ApprovalRequest {
  id: string;
  requesterId: string;
  requesterName: string;
  requesterAvatar?: string;
  title: string;
  description: string;
  workflowType: number; // 1: LeaveRequest, 2: BudgetApproval, 3: ExpenseClaim, 4: DocumentSignOff
  status: number; // 1: Pending, 2: Approved, 3: Rejected, 4: Cancelled
  currentStepNumber: number;
  totalSteps: number;
  payloadJson: string;
  createdAt: string;
  completedAt?: string;
  steps: ApprovalStep[];
}

export interface DocumentItem {
  id: string;
  title: string;
  category: string;
  currentVersion: number;
  accessLevel: number;
  ownerName: string;
  latestFileName?: string;
  latestFileSize: number;
  createdAt: string;
  updatedAt?: string;
}

export interface NotificationItem {
  id: string;
  title: string;
  message: string;
  type: number;
  isRead: boolean;
  targetUrl?: string;
  createdAt: string;
}

export interface DashboardKpi {
  totalEmployees: number;
  totalProjects: number;
  activeTasks: number;
  completedTasks: number;
  pendingApprovals: number;
  totalDocuments: number;
  projectCompletionRate: number;
  departmentDistributions: { departmentName: string; employeeCount: number }[];
  monthlyActivities: { month: string; created: number; completed: number }[];
  recentApprovals: {
    id: string;
    title: string;
    requesterName: string;
    type: string;
    status: string;
    createdAt: string;
  }[];
}
