using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class GenerateScreen : DTNView
{
    public TextMeshProUGUI tittleText;
    public TMP_InputField inputField;
    public GameObject imageGen;
    public GameObject textGen;
    public Button uploadFile;
    public Button generate;
    public Button back;
    private OptionGenerate option;
    public Texture2D texture;
    public string imgPath;
    public override void Init()
    {
        AddOnclickAction();
    }

    private void AddOnclickAction()
    {
        back.onClick.AddListener(() =>
        {
            DTNWindow.FindTopWindow().ShowSubView<OptionGenerateScreen>();
            Hide();
        });

        uploadFile.onClick.AddListener(() =>
        {
            OpenGallery();
        });


        generate.onClick.AddListener(() =>
        {
            switch (option)
            {
                case OptionGenerate.TextToBlock2D:
                    MainController.Instance.GenerateTextToBlock2D(inputField.text);
                    break;
                case OptionGenerate.TextToBlock3D:
                    MainController.Instance.GenerateTextToBlock3D(inputField.text);
                    break;
                case OptionGenerate.ImageToBlock2D:
                    MainController.Instance.GenerateImageToBlock2D(texture);
                    break;
                case OptionGenerate.ImageToBlock3D:
                    MainController.Instance.GenerateImageToBlock3D(imgPath);
                    break;
            }
            Hide();
        });
    }

    public void OpenGallery()
    {
        if (NativeGallery.IsMediaPickerBusy())
        {
            return;
        }

        // Mở Native Gallery để chọn ảnh
        NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {

                texture = NativeGallery.LoadImageAtPath(path, 1024);
            }
        }, "Select an image", "image/*");
    }


    public void SetupOption(OptionGenerate option)
    {
        this.option = option;
        imageGen.SetActive(false);
        textGen.SetActive(false);
        switch (option)
        {
            case OptionGenerate.TextToBlock2D:
                tittleText.text = "Text To Block 2D";
                textGen.SetActive(true);
                break;
            case OptionGenerate.TextToBlock3D:
                tittleText.text = "Text To Block 3D";
                textGen.SetActive(true);
                break;
            case OptionGenerate.ImageToBlock2D:
                tittleText.text = "Image To Block 2D";
                imageGen.SetActive(true);
                break;
            case OptionGenerate.ImageToBlock3D:
                tittleText.text = "Image To Block 3D";
                imageGen.SetActive(true);
                break;
        }
    }
}
