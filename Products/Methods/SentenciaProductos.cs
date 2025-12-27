using Dapper;

namespace productos.Methods{
    public class SentenciaProductos{
        int? Page;
        int? Limit;
        string? Sort;
        string? Order;
        bool? Status;
        bool? Is_delete;
        string? Type;
        
        object[] valores = new object[7];

        string sentencia = "SELECT * FROM \"products\" WHERE 1=1 ";

        // Primary Constructor
        public SentenciaProductos(ProductsQueryDTO productsQueryDTO)
        {   
            Page = productsQueryDTO.Page;
            Limit = productsQueryDTO.Limit;
            Sort = productsQueryDTO.Sort;
            Order = productsQueryDTO.Order;
            Status = productsQueryDTO.Status;
            Is_delete = productsQueryDTO.isDeleted;
            Type = productsQueryDTO.Type;
        }

        public (string Sentencia, DynamicParameters Parametros, object[] valores) CrearSenentiaSQLProduct()
        {
            var parametros = new DynamicParameters();
            
            if(Status.HasValue)
            {
                sentencia += " AND status = @status ";
                parametros.Add("@status", Status);
                valores[0] = Status;
            }

            if(Is_delete.HasValue)
            {
                sentencia += " AND is_deleted = @is_deleted ";
                parametros.Add("@is_deleted", Is_delete);
                valores[1] = Is_delete;
            }
            
            if (!string.IsNullOrEmpty(Type))
            {
                sentencia += " AND type = @type ";
                parametros.Add("@type", Type);
                valores[2] = Type;
            }

            //Verifiquemos que el orden no sea null ya que safeOrder tiene valor asc por defecto.
            if (!string.IsNullOrEmpty(Sort) && !string.IsNullOrEmpty(Order))
            {
                sentencia += $" ORDER BY {Sort} {Order} ";
                valores[3] = Sort;
                valores[4] = Order;
            }

            if(Limit.HasValue && Page.HasValue)
            {
                // Paginación
                sentencia += " LIMIT @limit OFFSET @offset ";
                parametros.Add("@limit", Limit);
                parametros.Add("@offset", (Page - 1) * Limit);
                valores[5] = Limit;
                valores[6] = Page;
            }
            return (sentencia, parametros, valores);
        }
    }
}