import type { Pagamento } from '../types/pagamento';

interface Props {
  pagamentos: Pagamento[];
  carregando: boolean;
}

function formatarValor(valor: number): string {
  return valor.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
}

function formatarData(data: string): string {
  return new Date(data).toLocaleString('pt-BR');
}

export function PagamentosTable({ pagamentos, carregando }: Props) {
  if (carregando) {
    return <p className="mensagem-estado">Carregando pagamentos...</p>;
  }

  if (pagamentos.length === 0) {
    return <p className="mensagem-estado">Nenhum pagamento encontrado.</p>;
  }

  return (
    <table className="tabela-pagamentos">
      <thead>
        <tr>
          <th>ID Transação</th>
          <th>ID Contrato</th>
          <th>Valor</th>
          <th>Data Pagamento</th>
          <th>Status</th>
          <th>Recebido em</th>
        </tr>
      </thead>
      <tbody>
        {pagamentos.map((p) => (
          <tr key={p.id_transacao}>
            <td>{p.id_transacao}</td>
            <td>{p.id_contrato}</td>
            <td>{formatarValor(p.valor)}</td>
            <td>{formatarData(p.data_pagamento)}</td>
            <td>{p.status}</td>
            <td>{formatarData(p.recebido_em)}</td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}
