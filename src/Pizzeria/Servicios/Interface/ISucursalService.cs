public interface ISucursalService
{
    Task<IEnumerable<Sucursal>> VerActivasAsync();
    Task<IEnumerable<SucursalReparto>> VerRepartoAsync();
    Task<int> RegistrarAsync(Sucursal sucursal);
}