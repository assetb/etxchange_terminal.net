using altaik.baseapp.ext;

namespace DocumentFormation.model
{
    public enum DocumentTypeEnum
    {
        [Name("Техническая спецификация")]
        TechSpecs,

        [Name("Квалификационные требования")]
        Qualifications,

        [Name("Проект договора")]
        Agreement,

        [Name("Заявка на проведение")]
        Application
    }
}
