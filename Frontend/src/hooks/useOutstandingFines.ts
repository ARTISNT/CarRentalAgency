import { useQuery } from '@tanstack/react-query';
import { rentalApi } from '../api/endpoints';

export const OUTSTANDING_FINES_QUERY_KEY = (userId?: string) =>
  ['outstanding-fines', userId] as const;

export function useOutstandingFines(userId?: string) {
  return useQuery({
    queryKey: OUTSTANDING_FINES_QUERY_KEY(userId),
    queryFn: () => rentalApi.getOutstandingFines(userId!),
    enabled: !!userId,
  });
}
