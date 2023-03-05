using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;
using UnityEngine.UI;
using TMPro;

public class MapDisplay : MonoBehaviour
{
	public Color oceanColor;
	private Color[] regionColors = new Color[16];
	public Color lowRainfall;
	public Color highRainfall;
	public Renderer textureRender;
	public Text text;

	private int[,] regionsMap;
	private Dictionary<string, List<float>> rainfallData;
	private int currentTimeStampIndex = 0;
	private float maxRainfallValue, minRainfallValue;

	// Start is called before the first frame update
	void Start()
    {
		regionColors[0] = oceanColor;
		regionsMap = ReadRegionsMap(Application.dataPath + @"\CSVs\ukregions.csv");
		Debug.Log(regionsMap);
		//int[,] regionsMap = AssignRegionsToMap(Application.dataPath + @"\EnglandIrelandBWMap.jpg");

		string[] rainfallText = File.ReadAllLines(Application.dataPath + @"\rainfall.txt");
		rainfallData = new Dictionary<string, List<float>>();
		maxRainfallValue = 0;
		minRainfallValue = float.MaxValue;
		foreach(string line in rainfallText)
        {
			string[] splitLine = line.Split("	");
			string timestamp = splitLine[0];
			if (splitLine[2] != "NaN")
            {
				maxRainfallValue = Math.Max(maxRainfallValue, float.Parse(splitLine[2]));
				minRainfallValue = Math.Min(minRainfallValue, float.Parse(splitLine[2]));
				if (rainfallData.ContainsKey(timestamp))
				{
					rainfallData[timestamp].Add(float.Parse(splitLine[2]));
				}
				else
				{
					List<float> newList = new List<float>();
					newList.Add(float.Parse(splitLine[2]));
					rainfallData.Add(timestamp, newList);
				}
			}
        }

		maxRainfallValue -= minRainfallValue;
		colorUpdate();
	}
	public float timestampLength = 1.0f;
	private int option = 0;
	private bool paused = false;
	private float currentTime = 0.0f;

	void Update()
	{
		if (!paused)
			currentTime += Time.deltaTime;

		if (Input.GetKeyDown("space"))
			paused = !paused;

		if (currentTime > timestampLength)
		{
			currentTimeStampIndex = (currentTimeStampIndex + 1) % GetRainfallTimestamps().Count;
			colorUpdate();
			currentTime = currentTime % timestampLength;
			text.text = GetRainfallTimestamps()[currentTimeStampIndex];
		}
	}

	public Texture2D GetRegionsTexture(Color[] colors, int[,] map) {
		int width = map.GetLength(0);
		int height = map.GetLength(1);

		Color[] colourMap = new Color[width * height];
		//Debug.Log(colourMap.Length);
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				//Debug.Log((y * width) + x);
				colourMap[(y * width) + x] = colors[map[x,y]];
			}
		}

		return TextureFromColourMap(colourMap, width, height);
	}

    public static Texture2D GetTexture() {
		int width = 100;
		int height = 100;

		Color[] colourMap = new Color[width * height];
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				colourMap [y * width + x] = Color.white;
			}
		}

		return TextureFromColourMap (colourMap, width, height);
	}

	public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height) {
		Texture2D texture = new Texture2D (width, height);
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.SetPixels(colourMap);
		texture.Apply();
		return texture;
	}

	public void DrawTexture(Texture2D texture) {
		textureRender.sharedMaterial.mainTexture = texture;
	}

	private int[,] ReadRegionsMap(string path) {
		int width = -1;
		int height = 0;

		int[,] map;

		using(var reader = new StreamReader(path))
    	{
        	List<float> values = new List<float>();

        	while (!reader.EndOfStream)
        	{
				var line = reader.ReadLine();
        	    string[] strs = line.Split(',');

				width = strs.Length;
				height++;

				foreach (string str in strs) {
					try
        			{
						values.Add(Int32.Parse(str));	
        			}
        			catch (FormatException e)
        			{
        			    values.Add(0);
        			}
				}
        	}

			map = new int[width,height];

			float[] regionColorsForMatching = { 255, 153, 65.025f, 182.07f, 219, 127, 222, 191, 172.89f, 140, 102, 84, 64, 170, 48, 77 };
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					//values[i, j] = pixColor.r;
					for (int c = 0; c < regionColorsForMatching.Length; c++)
					{
						//Debug.Log(pixColor.r);
						if (Math.Abs(regionColorsForMatching[c] - values[i + (j * width)]) < 0.5f)
						{
							//Debug.Log("color matched!");
							map[i, j] = c;
							break;
						}
					}
				}
			}

		}

		return map;
	}

	public List<string> GetRainfallTimestamps()
    {
		return new List<string>(rainfallData.Keys);
	}

	public void SetDropdownOption(int opt)
    {
		currentTimeStampIndex = opt;
		colorUpdate();
    }

	void colorUpdate()
    {
		string currentTimeStamp = GetRainfallTimestamps()[currentTimeStampIndex];
		List<float> values = rainfallData[currentTimeStamp];

		for (int i = 0; i < values.Count; i++)
		{
			float lerpValue = (values[i] - minRainfallValue) / maxRainfallValue;
			regionColors[i + 1] = Color.Lerp(lowRainfall, highRainfall, lerpValue);
		}
		DrawTexture(GetRegionsTexture(regionColors, regionsMap));
	}
}
