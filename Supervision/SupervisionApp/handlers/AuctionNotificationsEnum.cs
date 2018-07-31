using altaik.baseapp.ext;

namespace SupervisionApp
{
    public enum AuctionNotificationsEnum
    {
        [Name("Сегодня аукцион.")]
        AuctionStarted,

        [Name("Подходит срок отправки биржевого обеспечения на биржу.")]
        ExchangeProvisionDeadline,

        [Name("Подходит срок отправки списка допущенных поставщиков на биржу.")]
        ApplicantsDeadline,

        [Name("Период подачи заявок поставщиками.")]
        OrderDeadline
    }
}
