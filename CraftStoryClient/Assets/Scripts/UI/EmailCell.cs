using UnityEngine;
using UnityEngine.UI;

public class EmailCell : UIBase
{
    Text Title { get => FindChiled<Text>("Text"); }
    Text Time { get => FindChiled<Text>("Time"); }
    Button Btn { get => GetComponent<Button>(); }
    Image inObj { get => FindChiled<Image>("Image"); }

    EmailLG.EmailCellRP data;
    public EmailLG.EmailCellRP Data { get => data; }

    private void Awake()
    {
        Btn.onClick.AddListener(() =>
        {
            var ui = UICtl.E.OpenUI<EmailDetailsUI>(UIType.EmailDetails, UIOpenType.BeforeClose);
            ui.Set(this);
        });
    }

    public void Set(EmailLG.EmailCellRP data)
    {
        this.data = data;

        Title.text = data.title;
        Time.text = data.created_at.ToString("yyyy/MM/dd");

        if (data.IsAlreadyRead)
            Read();

        inObj.gameObject.SetActive(data.IsInObject);
    }

    public void Read()
    {
        var image = GetComponent<Image>();
        image.color = Color.grey;
    }
}