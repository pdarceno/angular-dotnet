using CsvHelper.Configuration;
using backend.DTOs;

namespace backend.DTOs
{
    public sealed class OrderCsvDtoMap : ClassMap<OrderCsvDto>
    {
        public OrderCsvDtoMap()
        {
            Map(m => m.OrderId).Name("order_id");
            Map(m => m.Date).Name("date");
            Map(m => m.Time).Name("time");
        }
    }
}
