import type { FiltrosPagamento } from '../types/pagamento';

interface Props {
  filtros: FiltrosPagamento;
  onChange: (filtros: FiltrosPagamento) => void;
  onAtualizar: () => void;
}

export function FiltrosPagamentos({ filtros, onChange, onAtualizar }: Props) {
  return (
    <div className="filtros">
      <div className="filtro-campo">
        <label htmlFor="filtro-status">Status</label>
        <select
          id="filtro-status"
          value={filtros.status ?? ''}
          onChange={(e) => onChange({ ...filtros, status: e.target.value || undefined })}
        >
          <option value="">Todos</option>
          <option value="Sucesso">Sucesso</option>
          <option value="Erro">Erro</option>
        </select>
      </div>

      <div className="filtro-campo">
        <label htmlFor="filtro-contrato">ID do Contrato</label>
        <input
          id="filtro-contrato"
          type="text"
          placeholder="ex: CT-9001"
          value={filtros.id_contrato ?? ''}
          onChange={(e) => onChange({ ...filtros, id_contrato: e.target.value || undefined })}
        />
      </div>

      <button type="button" onClick={onAtualizar}>
        Atualizar
      </button>
    </div>
  );
}
