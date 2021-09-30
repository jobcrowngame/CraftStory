using Amazon;
using Amazon.CognitoIdentity;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.IO;

public class AWSS3Mng : MonoBehaviour
{
    public static AWSS3Mng E
    {
        get
        {
            if (entity == null)
                entity = UICtl.E.CreateGlobalObject<AWSS3Mng>();

            return entity;
        }
    }
    private static AWSS3Mng entity;

    private string IdentityPoolId = "ap-northeast-1:073ecf65-6c35-42df-8607-5a304f7a9006";
    private string objectName = "test.TXT";
    private string S3BucketName = "j-de";

    private RegionEndpoint _CognitoIdentityRegion
    {
        get { return RegionEndpoint.GetBySystemName(S3Region); }
    }
    public string S3Region = RegionEndpoint.APNortheast1.SystemName;
    private RegionEndpoint _S3Region
    {
        get { return RegionEndpoint.GetBySystemName(S3Region); }
    }

    private IAmazonS3 _s3Client;
    private AWSCredentials _credentials;

    private AWSCredentials Credentials
    {
        get
        {
            if (_credentials == null)
                _credentials = new CognitoAWSCredentials(IdentityPoolId, _CognitoIdentityRegion);
            return _credentials;
        }
    }

    private IAmazonS3 Client
    {
        get
        {
            if (_s3Client == null)
            {
                _s3Client = new AmazonS3Client(Credentials, _S3Region);
            }
            //test comment
            return _s3Client;
        }
    }

    /// <summary>
    /// 初期化
    /// </summary>
    /// <returns></returns>
    public IEnumerator InitInitCoroutine()
    {
        UnityInitializer.AttachToGameObject(gameObject);

        AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
        AWSConfigs.LoggingConfig.LogTo = LoggingOptions.UnityLogger;
        AWSConfigs.LoggingConfig.LogResponses = ResponseLoggingOption.Always;
        AWSConfigs.LoggingConfig.LogMetrics = true;
        AWSConfigs.CorrectForClockSkew = true;

        yield return null;
    }

    /// <summary>
    /// 画像をアップロード
    /// </summary>
    public void UploadTexture2D(Texture2D texture, string fileName, Action successCallback = null, Action failureCallback = null)
    {
        //Texture2Dを作成
        byte[] data = texture.EncodeToPNG();

        MemoryStream stream = new MemoryStream(data.Length);
        stream.Write(data, 0, data.Length);
        stream.Seek(0, SeekOrigin.Begin);

        var request = new PutObjectRequest()
        {
            BucketName = S3BucketName,
            Key = fileName,
            InputStream = stream,
            CannedACL = S3CannedACL.Private
        };

        Client.PutObjectAsync(request, (responseObj) =>
        {
            if (responseObj.Exception == null)
            {
                if (successCallback != null) successCallback();
            }
            else
            {
                if (failureCallback != null) failureCallback();
                Logger.Error("S3 Upload Failure:"+responseObj.Exception.Message);
            }
        });
    }

    /// <summary>
    /// 画像をダウンロード
    /// </summary>
    public void DownLoadTexture2D(Image img, string fileName)
    {
        GetObjectRequest request = new GetObjectRequest
        {
            BucketName = S3BucketName,
            Key = fileName,
        };

        Client.GetObjectAsync(request, (responseObject) =>
        {
            try
            {
                if (responseObject == null || responseObject.Response == null)
                {
                    Logger.Warning("S3 responseObject.Response is null!!!");
                    return;
                }

                MemoryStream stream = new MemoryStream();
                responseObject.Response.ResponseStream.CopyTo(stream);
                byte[] data = stream.ToArray();

                Texture2D tex = new Texture2D(1, 1);
                tex.LoadImage(data);
                tex.Apply();

                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
                if (img != null) img.sprite = sprite;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        });
    }
}