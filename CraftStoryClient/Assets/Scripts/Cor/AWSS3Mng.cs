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
using Amazon.Runtime.Internal;

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
    private string S3BucketName = "j-de";
    private string S3BucketNameHomeData = "j-de/home";
    private string S3BucketNameLoginBonus = "j-de/loginbonus";

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
        try
        {
            UnityInitializer.AttachToGameObject(gameObject);

            AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
            AWSConfigs.LoggingConfig.LogTo = LoggingOptions.UnityLogger;
            AWSConfigs.LoggingConfig.LogResponses = ResponseLoggingOption.Always;
            AWSConfigs.LoggingConfig.LogMetrics = true;
            AWSConfigs.CorrectForClockSkew = true;
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }

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

        try
        {
            Client.PutObjectAsync(request, (responseObj) =>
            {
                if (responseObj.Exception == null)
                {
                    if (successCallback != null) successCallback();
                }
                else
                {
                    if (failureCallback != null) failureCallback();
                    Logger.Error("S3 Upload Failure:" + responseObj.Exception.Message);
                }
            });
        }
        catch (HttpErrorResponseException ex)
        {
            Logger.Error("HttpErrorResponseException:" + ex);
        }
        catch (AmazonS3Exception ex)
        {
            Logger.Error("AmazonS3Exception:" + ex);
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }
    }

    /// <summary>
    /// 画像をダウンロード
    /// </summary>
    public void DownLoadTexture2D(Image img, string fileName, string bucetName)
    {
        DownLoadTexture2D(img, fileName, null, null, bucetName);
    }
    public void DownLoadTexture2D(Image img, string fileName, Action successCallback = null, Action failureCallback = null, string bucetName = "")
    {
        GetObjectRequest request = new GetObjectRequest
        {
            BucketName = string.IsNullOrEmpty(bucetName) ? S3BucketName : bucetName,
            Key = fileName,
        };

        try
        {
            Client.GetObjectAsync(request, (responseObject) =>
            {
                if (responseObject == null || responseObject.Response == null)
                {
                    if (failureCallback != null) failureCallback();
                    return;
                }

                if (responseObject.Exception == null)
                {
                    MemoryStream stream = new MemoryStream();
                    responseObject.Response.ResponseStream.CopyTo(stream);
                    byte[] data = stream.ToArray();

                    Texture2D tex = new Texture2D(1, 1);
                    tex.LoadImage(data);
                    tex.Apply();

                    Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
                    if (img != null) img.sprite = sprite;
                    if (successCallback != null) successCallback();
                }
                else
                {
                    if (failureCallback != null) failureCallback();
                    Logger.Error("S3 DownLoad Object Failure:" + responseObject.Exception.Message);
                }
            });
        }
        catch (HttpErrorResponseException ex)
        {
            Logger.Error("HttpErrorResponseException:" + ex);
        }
        catch (AmazonS3Exception ex)
        {
            Logger.Error("AmazonS3Exception:" + ex);
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }
    }

    /// <summary>
    /// マップデータをセーブ
    /// </summary>
    /// <param name="strData"></param>
    public void SaveHomeData(string key, string strData, Action successCallback = null, Action failureCallback = null)
    {
        byte[] data = System.Text.Encoding.ASCII.GetBytes(strData);

        MemoryStream stream = new MemoryStream(data.Length);
        stream.Write(data, 0, data.Length);
        stream.Seek(0, SeekOrigin.Begin);

        var request = new PutObjectRequest()
        {
            BucketName = S3BucketNameHomeData,
            Key = key,
            InputStream = stream,
            CannedACL = S3CannedACL.Private
        };

        try
        {
            Client.PutObjectAsync(request, (responseObj) =>
            {
                if (responseObj.Exception == null)
                {
                    if (successCallback != null) successCallback();
                }
                else
                {
                    if (failureCallback != null) failureCallback();
                    Logger.Error("S3 Upload Failure:" + responseObj.Exception.Message);
                }
            });
        }
        catch (HttpErrorResponseException ex)
        {
            Logger.Error("HttpErrorResponseException:" + ex);
        }
        catch (AmazonS3Exception ex)
        {
            Logger.Error("AmazonS3Exception:" + ex);
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="successCallback"></param>
    /// <param name="failureCallback"></param>
    public void LoadHomeData(string key, Action<string> successCallback, Action failureCallback = null)
    {
        GetObjectRequest request = new GetObjectRequest
        {
            BucketName = S3BucketNameHomeData,
            Key = key,
        };

        try
        {
            Client.GetObjectAsync(request, (responseObject) =>
            {
                if (responseObject.Exception == null)
                {
                    MemoryStream stream = new MemoryStream();
                    responseObject.Response.ResponseStream.CopyTo(stream);
                    byte[] data = stream.ToArray();

                    string text = System.Text.Encoding.ASCII.GetString(data);
                    if (successCallback != null) successCallback(text);
                }
                else
                {
                    if (failureCallback != null) failureCallback();
                }
            });
        }
        catch (HttpErrorResponseException ex)
        {
            Logger.Error("HttpErrorResponseException:" + ex);
        }
        catch (AmazonS3Exception ex)
        {
            Logger.Error("AmazonS3Exception:" + ex);
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }
    }

    /// <summary>
    /// ログインボーナスIconロード
    /// </summary>
    public void LoadLoginBonusTexture2D(Image img, string fileName)
    {
        DownLoadTexture2D(img, fileName, null, null, S3BucketNameLoginBonus);
    }
}