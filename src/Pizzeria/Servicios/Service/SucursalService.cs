public class SucursalService : ISucursalService
{
    private readonly ISucursalRepository _repo;
    public SucursalService(ISucursalRepository repo) => _repo = repo;

    public Task<IEnumerable<Sucursal>> VerActivasAsync() => _repo.ObtenerActivasAsync();
    public Task<IEnumerable<SucursalReparto>> VerRepartoAsync() => _repo.ObtenerRepartoAsync();

    public async Task<int> RegistrarAsync(Sucursal s)
    {
        if (string.IsNullOrWhiteSpace(s.Direccion))
            throw new ArgumentException("La dirección es obligatoria.");

        if (s.Latitud is null or < -90 or > 90 || s.Longitud is null or < -180 or > 180)
            throw new ArgumentException("Marcá la ubicación de la sucursal en el mapa.");

        s.Direccion = s.Direccion.Trim();
        s.Telefono = s.Telefono?.Trim() ?? string.Empty;

        return await _repo.CrearAsync(s);
    }
}