namespace APIs_Graduation.DTOs
{
    public class PaymobWebhookData
    {
        public int OrderId { get; set; }  // معرّف الطلب في Paymob
        public int MerchantOrderId { get; set; }  // معرّف الطلب عندنا
        public bool Success { get; set; }  // هل تمت العملية بنجاح؟
    }


}
