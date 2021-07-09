using UnityEngine;
using UnityEngine.UI;

public class EmailCell : UIBase
{
    Text Title { get => FindChiled<Text>("Text"); }
    Text Time { get => FindChiled<Text>("Time"); }
    Button Btn { get => GetComponent<Button>(); }

    EmailRP data;
    public EmailRP Data { get => data; }

    private void Awake()
    {
        Btn.onClick.AddListener(() =>
        {
            var ui = UICtl.E.OpenUI<EmailDetailsUI>(UIType.EmailDetails, UIOpenType.BeforeClose);
            ui.Set(this);
        });
    }

    public void Set(EmailRP data)
    {
        this.data = data;

        Title.text = "From: " + data.title;
        Time.text = data.created_at.ToString();

        if (data.IsAlreadyRead)
            Read();
    }

    public void Read()
    {
        var image = GetComponent<Image>();
        image.color = Color.grey;
    }
}