using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance {  get; private set; }

    public static void Initialize()
    {
        GameObject gameObject = new GameObject("ResourceManager");
        Instance = gameObject.AddComponent<ResourceManager>();
    }

    public UnityEvent<string> onError { get; set; }

    private string[] assetList = null;
    private Dictionary<string, Sprite> mSprites = new Dictionary<string, Sprite>();
    private Dictionary<string, AsyncOperationHandle<Sprite>> mRunningLoaders = new Dictionary<string, AsyncOperationHandle<Sprite>>();
    
    public bool isSpriteAvailable(string pAsset)
    {
        return mSprites.ContainsKey(pAsset);
    }

    public Sprite getSprite(string pSpritename)
    {
        return mSprites[pSpritename];
    }

    public void Start()
    {
        AsyncOperationHandle<TextAsset> assetDB = Addressables.LoadAssetAsync<TextAsset>("assetlist");
        assetDB.Completed += (task) =>
        {
            TextAsset assetDB = task.Result;
            assetList = assetDB.text.Replace("\r\n", "\n").Split('\n');
            foreach (string asset in assetList)
            {
                if (asset.Trim().Length > 0)
                {
                    mRunningLoaders.Add(asset, Addressables.LoadAssetAsync<Sprite>(asset));
                }
            }
        };
    }

    public void Update()
    {
        int i = 0;
        while (i < mRunningLoaders.Count)
        {
            KeyValuePair<string, AsyncOperationHandle<Sprite>> entry = mRunningLoaders.ElementAt(i);
            string assetToLoad = entry.Key;
            AsyncOperationHandle<Sprite> loader = entry.Value;

            if (loader.IsDone)
            {
                if (loader.Status == AsyncOperationStatus.Succeeded)
                {
                    mSprites.Add(assetToLoad, loader.Result);
                }
                else
                {
                    onError.Invoke(assetToLoad);
                }

                Addressables.Release(loader);
                mRunningLoaders.Remove(assetToLoad);
            }
            else
            {
                i++;
            }
        }
    }

    private void OnErrorLoadingAsset(string mAssetName)
    {
        Debug.LogWarning("Unable to load asset: " +  mAssetName);
    }
}
