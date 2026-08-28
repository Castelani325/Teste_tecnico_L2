import type { Pagamento, FiltrosPagamento } from '../types/pagamento';

const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5001';

export async function listarPagamentos(filtros: FiltrosPagamento): Promise<Pagamento[]> {
  const params = new URLSearchParams();

  if (filtros.status) {
    params.set('status', filtros.status);
  }
  if (filtros.id_contrato) {
    params.set('id_contrato', filtros.id_contrato);
  }

  const query = params.toString();
  const url = `${API_URL}/pagamentos${query ? `?${query}` : ''}`;

  const response = await fetch(url);

  if (!response.ok) {
    throw new Error(`Erro ao buscar pagamentos: ${response.status}`);
  }

  return response.json();
}
