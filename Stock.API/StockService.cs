using Common.Shared.DTOs;
using System.Net;

namespace Stock.API
{
    public class StockService
    {
        public Dictionary<int,int> GetProductStockList()
        {
            Dictionary<int, int> productStockList = new();
            productStockList.Add(1, 10);
            productStockList.Add(2, 20);
            productStockList.Add(3,30);

            return productStockList;
        }

        public ResponseDto<StockCheckAndPaymentProcessResponseDto> CheckAndPaymentProcess(StockCheckAndPaymentProcessRequestDto request)
        {
            var productStockList = GetProductStockList();
            var stockStatus = new List<(int productId, bool hasStockExist)>();

            foreach (var orderItem in request.OrderItems)
            {
                var hasExistStock = productStockList.Any(x => x.Key == orderItem.ProductId && x.Value>=orderItem.Count);
                stockStatus.Add((orderItem.ProductId, hasExistStock));
            }

            if (stockStatus.Any(x => x.hasStockExist == false))
            {
                return ResponseDto<StockCheckAndPaymentProcessResponseDto>.Fail(StatusCodes.Status400BadRequest,"Stock Yetersiz.");
            }

            // Payment Süreci yapılacak


            return ResponseDto<StockCheckAndPaymentProcessResponseDto>.Success(StatusCodes.Status200OK,new StockCheckAndPaymentProcessResponseDto()
            {
                Description="Stock rezerve edildi."
            });


        }
    }
}
