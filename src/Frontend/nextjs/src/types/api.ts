// ============================================================
// EChamado API Types — espelho dos DTOs do backend .NET 10
// ============================================================

// --- Envelope de resposta ---
// BaseResult<void> = resposta sem dados (apenas success + message)
// BaseResult<T> = resposta com dados (success + message + data)
export interface BaseResult<T = void> {
  success: boolean;
  message: string;
  data: T;
}

export interface BaseResultList<T> {
  success: boolean;
  message: string;
  data: T[];
  pagedResult: PagedResult;
}

export interface PagedResult {
  currentPage: number;
  pageCount: number;
  pageSize: number;
  rowCount: number;
  firstRowOnPage: number;
  lastRowOnPage: number;
}

// --- Orders ---
export interface OrderViewModel {
  id: string;
  title: string;
  description: string;
  evaluation: number | null;
  openingDate: string;
  closingDate: string | null;
  dueDate: string | null;
  statusId: string;
  statusName: string;
  typeId: string;
  typeName: string;
  categoryId: string;
  categoryName: string | null;
  subCategoryId: string | null;
  subCategoryName: string | null;
  departmentId: string;
  departmentName: string | null;
  requestingUserId: string;
  requestingUserEmail: string;
  responsibleUserId: string;
  responsibleUserEmail: string;
  createdAt: string;
  updatedAt: string | null;
  comments: CommentResponse[];
  isOverdue: boolean;
}

export interface OrderListViewModel {
  id: string;
  title: string;
  openingDate: string;
  closingDate: string | null;
  dueDate: string | null;
  statusName: string;
  typeName: string;
  departmentName: string | null;
  requestingUserEmail: string;
  responsibleUserEmail: string;
  isOverdue: boolean;
}

export interface CreateOrderRequest {
  title: string;
  description: string;
  typeId: string;
  categoryId?: string;
  subCategoryId?: string;
  departmentId?: string;
  dueDate?: string;
  requestingUserId: string;
  requestingUserEmail: string;
}

export interface UpdateOrderRequest {
  id: string;
  title: string;
  description: string;
  typeId: string;
  categoryId?: string;
  subCategoryId?: string;
  departmentId?: string;
  dueDate?: string;
}

export interface CloseOrderRequest {
  orderId: string;
  evaluation?: number;
}

export interface ChangeStatusRequest {
  orderId: string;
  statusTypeId: string;
}

export interface AssignOrderRequest {
  orderId: string;
  assignedToUserId: string;
}

export interface SearchOrdersParameters {
  pageIndex?: number;
  pageSize?: number;
  title?: string;
  description?: string;
  statusTypeId?: string;
  typeId?: string;
  departmentId?: string;
  categoryId?: string;
  subCategoryId?: string;
  startDate?: string;
  endDate?: string;
  createdByUserId?: string;
  assignedToUserId?: string;
  isOverdue?: boolean;
}

export interface DashboardStatsResponse {
  totalTickets: number;
  myTickets: number;
  assignedToMe: number;
  overdueTickets: number;
}

// --- Comments ---
export interface CommentResponse {
  id: string;
  text: string;
  orderId: string;
  userId: string;
  userEmail: string;
  isInternal: boolean;
  createdAt: string;
}

export interface CreateCommentRequest {
  orderId: string;
  description: string;
  userId: string;
  userEmail: string;
  isInternal?: boolean;
}

// --- Categories ---
export interface CategoryResponse {
  id: string;
  name: string;
  description: string;
  subCategories: SubCategoryResponse[];
}

export interface SubCategoryResponse {
  id: string;
  name: string;
  description: string;
  categoryId: string;
  categoryName: string | null;
}

export interface CreateCategoryRequest {
  name: string;
  description: string;
}

export interface UpdateCategoryRequest {
  name: string;
  description: string;
}

export interface CreateSubCategoryRequest {
  name: string;
  description: string;
  categoryId: string;
}

export interface UpdateSubCategoryRequest {
  name: string;
  description: string;
  categoryId: string;
}

// --- Departments ---
export interface DepartmentResponse {
  id: string;
  name: string;
  description: string;
}

export interface CreateDepartmentRequest {
  name: string;
  description: string;
}

export interface UpdateDepartmentRequest {
  name: string;
  description: string;
}

// --- OrderTypes ---
export interface OrderTypeResponse {
  id: string;
  name: string;
  description: string;
}

export interface CreateOrderTypeRequest {
  name: string;
  description: string;
}

export interface UpdateOrderTypeRequest {
  name: string;
  description: string;
}

// --- StatusTypes ---
export interface StatusTypeResponse {
  id: string;
  name: string;
  description: string;
}

export interface CreateStatusTypeRequest {
  name: string;
  description: string;
}

export interface UpdateStatusTypeRequest {
  name: string;
  description: string;
}

// --- Users ---
export interface UserResponse {
  id: string;
  email: string;
  userName: string;
  firstName?: string;
  lastName?: string;
  emailConfirmed: boolean;
  phoneNumberConfirmed: boolean;
  twoFactorEnabled: boolean;
  lockoutEnd?: string;
  lockoutEnabled: boolean;
  accessFailedCount: number;
  departmentId?: string;
  createdAt: string;
  updatedAt?: string;
  roles: string[];
}

// --- Roles ---
export interface RoleResponse {
  id: string;
  name: string;
  description?: string;
  permissions: string[];
}

export interface CreateRoleRequest {
  name: string;
  description: string;
  permissions: string[];
}

export interface UpdateRoleRequest {
  name: string;
  description: string;
  permissions: string[];
}

// --- AI ---
export interface NLToGridifyResult {
  success: boolean;
  gridifyQuery: string;
  originalQuery: string;
  entityName: string;
  provider?: string;
  model?: string;
  fromCache: boolean;
  responseTimeMs: number;
  tokensUsed: number;
  errorMessage?: string;
}

// --- Auth ---
export interface UserInfo {
  isAuthenticated: boolean;
  userName: string;
  email: string;
  userId: string;
  roles: string[];
}

// --- SLA ---
export interface SlaStatsResponse {
  totalOpen: number;
  overdue: number;
  atRisk: number;
  onTime: number;
  closedOnTime: number;
  closedLate: number;
  complianceRate: number;
}

// --- Team ---
export interface TeamStatsResponse {
  totalAgents: number;
  totalOpenTickets: number;
  totalOverdueTickets: number;
  agents: AgentStats[];
}

export interface AgentStats {
  userId: string;
  email: string;
  openTickets: number;
  closedTickets: number;
  overdueTickets: number;
  totalTickets: number;
  avgResolutionDays: number;
}

// --- Reports ---
export interface ReportResult {
  reportType: string;
  generatedAt: string;
  totalRecords: number;
  summary: Record<string, unknown>;
}
