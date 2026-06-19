import apiClient from './client';
import type {
  LoginRequest,
  RegisterRequest,
  Car,
  AddCarRequest,
  UpdateCarRequest,
  RentalResponse,
  RentalListItem,
  CreateRentalRequest,
  RenewRentalRequest,
  EndRentalRequest,
  CancelRentalRequest,
  EstimatedPriceResponse,
  ContractResponse,
  CreateContractRequest,
  SignContractRequest,
  ChangeContractStatusRequest,
  ContractTemplate,
  CreateTemplateRequest,
  UpdateTemplateContentRequest,
  RenameTemplateRequest,
  UserResponse,
  UserWithPassport,
  PassportRequest,
  PaymentMethod,
  PaymentSummaryDto,
  PaymentTransactionDto,
  PayFineRequest,
  PayAdditionalRequest,
  PreviewFinalCostResponse,
  TemplateVariable,
} from '../types';

// ========== Auth ==========
export const authApi = {
  login: (data: LoginRequest) =>
    apiClient.post<string>('/User/login-user', data).then((r) => r.data),

  register: (data: RegisterRequest) =>
    apiClient.post('/User/register', data).then((r) => r.data),

  addPassport: (userId: string, data: PassportRequest) =>
    apiClient.post(`/User/add-passport/${userId}`, data).then((r) => r.data),
};

// ========== Users ==========
export const userApi = {
  getAll: () =>
    apiClient.get<UserResponse[]>('/User').then((r) => r.data),

  getById: (id: string) =>
    apiClient.get<UserWithPassport>(`/User/${id}`).then((r) => r.data),

  getWithPassport: (id: string) =>
    apiClient.get<UserWithPassport>(`/User/user-personal-info/${id}`).then((r) => r.data),

  deactivate: (userId: string) =>
    apiClient.put(`/User/deactivate-user/${userId}`).then((r) => r.data),

  activate: (userId: string) =>
    apiClient.put(`/User/activate-user/${userId}`).then((r) => r.data),

  delete: (userId: string) =>
    apiClient.delete(`/User/remove-user/${userId}`).then((r) => r.data),
};

// ========== Cars ==========
export const carApi = {
  getAll: () =>
    apiClient.get<Car[]>('/Car').then((r) => r.data),

  getAvailable: () =>
    apiClient.get<Car[]>('/Car/available').then((r) => r.data),

  getPublic: (carId: string) =>
    apiClient.get<Car>(`/Car/public-car/${carId}`).then((r) => r.data),

  getDetailed: (carId: string) =>
    apiClient.get<Car>(`/Car/detailed-car/${carId}`).then((r) => r.data),

  getMyRented: () =>
    apiClient.get<Car[]>('/Car/my-rented').then((r) => r.data),

  add: (data: AddCarRequest) =>
    apiClient.post('/Car/add-car', data).then((r) => r.data),

  update: (id: string, data: UpdateCarRequest) =>
    apiClient.put(`/Car/update-car/${id}`, data).then((r) => r.data),

  delete: (id: string) =>
    apiClient.delete(`/Car/delete-car/${id}`).then((r) => r.data),

  rent: (carId: string) =>
    apiClient.put(`/Car/rent/${carId}`).then((r) => r.data),

  return_: (carId: string) =>
    apiClient.put(`/Car/return/${carId}`).then((r) => r.data),

  markReturned: (carId: string) =>
    apiClient.put(`/Car/mark-returned/${carId}`).then((r) => r.data),

  break_: (carId: string) =>
    apiClient.put(`/Car/break/${carId}`).then((r) => r.data),

  sendToMaintenance: (carId: string) =>
    apiClient.put(`/Car/send-to-maintenance/${carId}`).then((r) => r.data),

  sendToRepair: (carId: string) =>
    apiClient.put(`/Car/send-to-repair/${carId}`).then((r) => r.data),

  completeMaintenance: (carId: string) =>
    apiClient.put(`/Car/complete-maintenance/${carId}`).then((r) => r.data),

  processReturn: (carId: string) =>
    apiClient.put(`/Car/process-return/${carId}`).then((r) => r.data),

  processReturnWithStatus: (carId: string, targetStatus: 'Available' | 'Maintenance' | 'Broken') =>
    apiClient
      .put(`/Car/process-return/${carId}`, { targetStatus })
      .then((r) => r.data),
};

// ========== Rentals ==========
export interface RentalListParams {
  carRenterId?: string;
  status?: string;
  dateFrom?: string;
  dateTo?: string;
  page?: number;
  pageSize?: number;
}

export const rentalApi = {
  getAll: (params?: RentalListParams) =>
    apiClient.get<RentalListItem[]>('/Rental/GetRentals', { params }).then((r) => r.data),

  getById: (id: string) =>
    apiClient.get<RentalResponse>(`/Rental/GetRental/${id}`).then((r) => r.data),

  create: (data: CreateRentalRequest) =>
    apiClient.post('/Rental/CreateRental', data).then((r) => r.data),

  calculateCost: (id: string, promoCode?: string | null) =>
    apiClient.post<EstimatedPriceResponse>(
      `/Rental/CalculateEstimatedCost/${id}`,
      { promoCode },
    ).then((r) => r.data),

  renew: (id: string, data: RenewRentalRequest) =>
    apiClient.put(`/Rental/RenewRental/${id}`, data).then((r) => r.data),

  end: (id: string, data: EndRentalRequest) =>
    apiClient.put(`/Rental/EndRental/${id}`, data).then((r) => r.data),

  previewFinalCost: (id: string, returnDate: string) =>
    apiClient
      .get<PreviewFinalCostResponse>(
        `/Rental/PreviewFinalCost/${id}?returnDate=${encodeURIComponent(returnDate)}`,
      )
      .then((r) => r.data),

  requestReturn: (id: string) =>
    apiClient.post(`/Rental/RequestReturn/${id}`).then((r) => r.data),

  cancel: (id: string, data?: CancelRentalRequest) =>
    apiClient.put(`/Rental/CancelRental/${id}`, data).then((r) => r.data),
};

// ========== Contracts ==========
export const contractApi = {
  getAll: () =>
    apiClient.get<ContractResponse[]>('/Contract/get-contracts').then((r) => r.data),

  getById: (id: string) =>
    apiClient.get<ContractResponse>(`/Contract/get-contract-${id}`).then((r) => r.data),

  getByRental: (rentalId: string) =>
    apiClient.get<ContractResponse[]>(`/Contract/get-contracts?RentalId=${rentalId}`).then((r) => r.data),

  create: (data: CreateContractRequest) =>
    apiClient.post('/Contract/create-contract', data).then((r) => r.data),

  sign: (data: SignContractRequest) =>
    apiClient.put('/Contract/sign-contract', data).then((r) => r.data),

  cancel: (contractId: string) =>
    apiClient.put('/Contract/cancel-contract', { contractId }).then((r) => r.data),

  changeStatus: (data: ChangeContractStatusRequest) =>
    apiClient.put('/Contract/change-status', data).then((r) => r.data),

  getPdfUrl: (id: string) =>
    `/api/Contract/get-contract-${id}/pdf`,
};

// ========== Contract Templates ==========
export const templateApi = {
  getAll: () =>
    apiClient.get<ContractTemplate[]>('/ContractTemplate/get-templates').then((r) => r.data),

  getById: (id: string) =>
    apiClient.get<ContractTemplate>(`/ContractTemplate/get-template-${id}`).then((r) => r.data),

  create: (data: CreateTemplateRequest) =>
    apiClient.post('/ContractTemplate/create-template', data).then((r) => r.data),

  updateContent: (data: UpdateTemplateContentRequest) =>
    apiClient.put('/ContractTemplate/update-content', data).then((r) => r.data),

  rename: (data: RenameTemplateRequest) =>
    apiClient.put('/ContractTemplate/rename', data).then((r) => r.data),

  activate: (id: string) =>
    apiClient.put(`/ContractTemplate/activate-${id}`).then((r) => r.data),

  deactivate: (id: string) =>
    apiClient.put(`/ContractTemplate/deactivate-${id}`).then((r) => r.data),

  getVariables: (documentType: string) =>
    apiClient
      .get<TemplateVariable[]>(`/ContractTemplate/variables?documentType=${encodeURIComponent(documentType)}`)
      .then((r) => r.data),
};

// ========== Payments ==========
export const paymentApi = {
  getMethods: () =>
    apiClient.get<PaymentMethod[]>('/Payments/methods').then((r) => r.data),

  pay: (rentalId: string, type: 'FullPayment' | 'Deposit') =>
    apiClient.post<string>(`/Payments/pay/${rentalId}?type=${type}`).then((r) => r.data),

  payFine: (rentalId: string, request: PayFineRequest) =>
    apiClient.post<string>(`/Payments/pay-fine/${rentalId}`, request).then((r) => r.data),

  payAdditional: (rentalId: string, request: PayAdditionalRequest) =>
    apiClient.post<string>(`/Payments/pay-additional/${rentalId}`, request).then((r) => r.data),

  payRemaining: (rentalId: string) =>
    apiClient.post<string>(`/Payments/pay-remaining/${rentalId}`).then((r) => r.data),

  refund: (rentalId: string) =>
    apiClient.post(`/Payments/refund/${rentalId}`).then((r) => r.data),

  confirm: (token: string) =>
    apiClient.post<{ rentalId: string }>(`/Payments/confirm?token=${token}`).then((r) => r.data),

  getTransactions: (rentalId: string) =>
    apiClient.get<PaymentTransactionDto[]>(`/Payments/transactions/by-rental/${rentalId}`).then((r) => r.data),

  getPaymentSummary: (rentalId: string) =>
    apiClient.get<PaymentSummaryDto>(`/Payments/payment-summary/${rentalId}`).then((r) => r.data),
};
