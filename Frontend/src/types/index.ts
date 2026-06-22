// ========== Auth ==========
export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  phoneNumber: string;
  password: string;
}

export interface AuthResponse {
  token: string;
}

export interface JwtPayload {
  nameid: string;
  email: string;
  role: string;
  permissions: string[];
}

// ========== User ==========
export type UserRole = 'Client' | 'Manager' | 'Admin';

export interface PassportDto {
  name: string;
  surname: string;
  patronymic: string;
  passportNumber: string;
  identityNumber: string;
  passportIssueDate: string;
  birthDate: string;
}

export interface UserResponse {
  id: string;
  isActive: boolean;
  emailVerified: boolean;
  password: string;
  phoneNumber: string;
  email: string;
  role: UserRole;
}

export interface UserWithPassport extends UserResponse {
  passportDto: PassportDto | null;
}

export interface PassportRequest {
  name: string;
  surname: string;
  patronymic: string;
  passportNumber: string;
  identityNumber: string;
  passportIssueDate: string;
  birthDate: string;
}

// ========== Car ==========
export type CarStatus = 'Available' | 'Rented' | 'Maintenance' | 'Broken' | 'Returned';
export type CarClass = 'Economy' | 'Standard' | 'Business' | 'Premium';
export type TransmissionType = 'Manual' | 'Automatic' | 'Variator' | 'Robotic';
export type DriveType = 'Fwd' | 'Rwd' | 'Awd' | 'FourByFour';
export type BodyStyle = 'Sedan' | 'Hatchback' | 'SUV' | 'Crossover' | 'StationWagon' | 'Minivan' | 'Van' | 'Coupe' | 'Convertible' | 'Pickup' | 'Limousine' | 'Roadster';
export type EngineType = 'Gasoline' | 'Diesel' | 'HybridGasoline' | 'HybridDiesel' | 'Electric';

export interface Car {
  id: string;
  brand: string;
  model: string;
  pricePerHour: number;
  class: CarClass;
  generation: string | null;
  isFacelift: boolean;
  variant: string | null;
  availabilityStatus: string;
  status: string;
  photoUrl: string | null;
  releaseDate?: string;
  mileage?: number;
  transmission?: string;
  driveType?: string;
  vinCode: string;
  licensePlate: string;
  color?: string;
  currentRenterId?: string | null;
  horsePower?: number;
  engineVolume?: number | null;
  powerReverse?: number | null;
  fuelCurrentLiters?: number | null;
  fuelCapacityLiters?: number | null;
  batteryCurrentKWh?: number | null;
  batteryCapacityKWh?: number | null;
}

export interface AddCarRequest {
  releaseDate: string;
  licensePlate: string;
  vinCode: string;
  color: string;
  model: string;
  brand: string;
  generation: string | null;
  isFacelift: boolean;
  variant: string | null;
  pricePerHour: number;
  CarClass: CarClass;
  photoUrl: string;
  fuelCurrentLiters: number | null;
  fuelCapacityLiters: number | null;
  batteryCurrentKWh: number | null;
  batteryCapacityKWh: number | null;
  mileage: number;
  bodyStyle: BodyStyle;
  transmissionType: TransmissionType;
  driveType: DriveType;
  engineType: EngineType;
  engineVolume: number | null;
  horsePower: number;
  powerReverse: number | null;
}

export interface UpdateCarRequest {
  releaseDate: string;
  licensePlate: string;
  vinCode: string;
  color: string;
  model: string;
  brand: string;
  generation: string | null;
  isFacelift: boolean;
  variant: string | null;
  pricePerHour: number;
  CarClass: CarClass;
  photoUrl: string;
  fuelCurrentLiters: number | null;
  fuelCapacityLiters: number | null;
  batteryCurrentKWh: number | null;
  batteryCapacityKWh: number | null;
  mileage: number;
  bodyStyle: BodyStyle;
  transmissionType: TransmissionType;
  driveType: DriveType;
  engineType: EngineType;
  engineVolume: number | null;
  horsePower: number;
  powerReverse: number | null;
}

// ========== Rental ==========
export type RentActivityStatus = 'AwaitingConfirmation' | 'Scheduled' | 'Active' | 'Completed' | 'Cancelled';

export interface RentalCarInfo {
  id: string;
  model: string;
  brand: string;
  generation: string | null;
  variant: string | null;
  isFacelift: boolean;
  licensePlate: string;
  pricePerHour: number;
  carClass: string;
}

export interface RentalRenterInfo {
  name: string;
  surName: string;
  patronymic: string | null;
  phoneNumber: string;
  email: string;
}

export interface RentalResponse {
  id: string;
  car: RentalCarInfo;
  renter: RentalRenterInfo;
  carRenterId: string;
  startDate: string;
  endDate: string;
  activityStatus: { name: string; id: number };
  totalCost: number;
  returnDate: string | null;
  returnRequestedAtUtc: string | null;
  depositRefundedAt: string | null;
  depositAmount: number;
  paidAmount: number;
  requiredAmount: number;
  remainingAmount: number;
  paymentStatus: string;
  fineOutstanding?: number;
  additionalOutstanding?: number;
  overpayment?: number;
  depositRefund?: number;
}

export interface RentalListItem {
  id: string;
  car: string;
  renter: string;
  renterId: string;
  phoneNumber: string;
  startDate: string;
  endDate: string;
  activityStatus: { name: string; id: number };
  totalCost: number;
  returnDate: string | null;
  returnRequestedAtUtc: string | null;
  depositRefundedAt: string | null;
  overpayment?: number;
}

export interface CreateRentalRequest {
  userId: string;
  carId: string;
  startDate: string;
  endDate: string;
  promoCode: string | null;
}

export interface RenewRentalRequest {
  newDate: string;
}

export interface EndRentalRequest {
  returnDate: string;
  mileage: number;
  fuelLevel: number;
  penaltyAmount: number;
  damageDescription: string | null;
}

export interface CancelRentalRequest {
  reason: string | null;
}

export interface MarkDepositRefundedRequest {
  note: string | null;
}

export interface OutstandingFinesResponse {
  outstandingFines: number;
}

export interface PreviewFinalCostResponse {
  finalCost: number;
  estimated: number;
  diff: number;
  depositAmount: number;
  refundAmount: number;
  currency: string;
}

export interface TemplateVariable {
  key: string;
  description: string;
  group: string;
  example: string;
}

export interface EstimatedPriceRequest {
  promoCode: string | null;
}

export interface EstimatedPriceResponse {
  estimatedAmount: number;
  depositAmount: number;
  discount: number;
  total: number;
}

// ========== Contract ==========
export type ContractStatus = 'AwaitingSignature' | 'Active' | 'Ended' | 'Cancelled';
export type DocumentType = 'Contract' | 'ReturnAct' | 'Addition';

export interface ContractResponse {
  id: string;
  clientFullName: string;
  car: string;
  startDate: string;
  endDate: string;
  estimatedPrice: number;
  status: ContractStatus;
  pdfPath: string | null;
  createdAt: string;
  rentalId: string;
  clientId: string;
}

export interface ContractAddition {
  id: string;
  content: string;
  createdAt: string;
}

export interface ContractReturnAct {
  returnDate: string;
  mileage: number;
  fuelLevel: number;
  damageDescription: string | null;
  penaltyAmount: number;
}

export interface ClientSnapshot {
  name: string;
  surname: string;
  patronymic: string;
  phoneNumber: string;
  email: string;
  passportNumber: string;
  identityNumber: string;
}

export interface ContractAutoSnapshot {
  brand: string;
  model: string;
  licensePlate: string;
  color: string;
  vinCode: string;
  year: number;
  class: CarClass;
  pricePerHour: number;
}

export interface ContractTemplateSnapshot {
  name: string;
  content: string;
  version: number;
  documentType: DocumentType;
}

export interface RentalSnapshot {
  startDate: string;
  endDate: string;
  promoCode: string | null;
  returnDate: string | null;
  activityStatus: RentActivityStatus;
  carRenterId: string;
}

export interface CreateContractRequest {
  clientId: string;
  rentalId: string;
  carId: string;
}

export interface SignContractRequest {
  id: string;
  signatureBase64: string;
}

export interface ChangeContractStatusRequest {
  contractId: string;
  status: ContractStatus;
}

// ========== Contract Templates ==========
export interface ContractTemplate {
  id: string;
  version: number;
  name: string;
  content: string;
  validFrom: string;
  createdOn: string;
  documentType: DocumentType;
  isActive: boolean;
}

export interface CreateTemplateRequest {
  name: string;
  content: string;
  documentType: DocumentType;
}

export interface UpdateTemplateContentRequest {
  id: string;
  content: string;
}

export interface RenameTemplateRequest {
  id: string;
  name: string;
}

// ========== Payment ==========
export type PaymentStatus = 'Pending' | 'Partially paid' | 'Paid' | 'Refunded' | 'Failed';
export type TransactionStatus = 'Pending' | 'Success' | 'Failed';
export type PaymentType =
  | 'Deposit'
  | 'FullPayment'
  | 'Fine'
  | 'Additional'
  | 'DepositRefund'
  | 'FineRefund';

export interface PaymentMethod {
  id: string;
  name: string;
  type: string;
}

export interface PaymentTransactionDto {
  id: string;
  amount: number;
  type: PaymentType;
  method: string;
  status: TransactionStatus;
  externalTransactionId: string;
  description: string | null;
  createdAt: string;
  completedAt: string | null;
  isRefunded: boolean;
}

export interface PaymentSummaryDto {
  rentalId: string;
  totalPrice: number;
  depositAmount: number;
  paidAmount: number;
  requiredAmount: number;
  remainingAmount: number;
  fineOutstanding: number;
  additionalOutstanding: number;
  paymentStatus: string;
  transactions: PaymentTransactionDto[];
}

export interface PayFineRequest {
  amount: number;
  reason: string;
}

export interface PayAdditionalRequest {
  amount: number;
  reason: string;
}

// ========== Common ==========
export interface ApiError {
  message: string;
  statusCode: number;
}

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}
