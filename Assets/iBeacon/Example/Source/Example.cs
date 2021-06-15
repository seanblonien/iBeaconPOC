using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using static Vector3D;
using static System.Math;

public enum BroadcastMode {
	send	= 0,
	receive	= 1,
	unknown = 2
}
public enum BroadcastState {
	inactive = 0,
	active	 = 1
}

internal class Example : MonoBehaviour {
	[SerializeField]
	private Text _statusText;

	[SerializeField]
	private GameObject _statusScreen;

	[SerializeField]
	private GameObject _menuScreen;

	[SerializeField]
	private Button _bluetoothButton;

	private Text _bluetoothText;

	/*** Beacon Properties ***/
	// Beacontype
	[SerializeField] //to access in editor, avoiding public varible declaration
	private Text txt_actualType;
	// Region
	[SerializeField]
	private Text txt_actualRegion;
	private string s_Region;
	// UUID, Namespace or Url
	[SerializeField]
	private Text txt_actualUUIDChar;
	[SerializeField]
	private Text txt_actualUUID;
	private string s_UUID;
	// Major/Minor or Instance
	[SerializeField]
	private Text txt_actualMajorChar;
	[SerializeField]
	private Text txt_actualMajor;
	private string s_Major;
	[SerializeField]
	private Text txt_actualMinorChar;
	[SerializeField]
	private Text txt_actualMinor;
	private string s_Minor;
	[SerializeField]
	private Text txt_location;
	[SerializeField]
	private Text txt_calcLatLong;

	/** Input **/
	// beacontype
	[SerializeField]
	private Dropdown InputDropdown;
	private BeaconType bt_PendingType;
	private BeaconType bt_Type;
	// Region
	[SerializeField]
	private InputField input_Region;
	// UUID, Namespace or Url
	[SerializeField]
	private Text txt_inputUUIDChar;
	[SerializeField]
	private InputField input_UUID;
	// Major/Minor or Instance
	[SerializeField]
	private Text txt_inputMajorChar;
	[SerializeField]
	private InputField input_Major;
	[SerializeField]
	private Text txt_inputMinorChar;
	[SerializeField]
	private InputField input_Minor;

	// Beacon BroadcastMode (Send, Receive)
	[SerializeField]
	private Text txt_BroadcastMode_ButtonText;
	[SerializeField]
	private Text txt_BroadcastMode_LabelText;
	private BroadcastMode bm_Mode;

	// Beacon BroadcastState (Start, Stop)
	[SerializeField]
	private Image img_ButtonBroadcastState;
	[SerializeField]
	private Text txt_BroadcastState_ButtonText;
	[SerializeField]
	private Text txt_BroadcastState_LabelText;
	private BroadcastState bs_State;

	// GameObject for found Beacons
	[SerializeField]
	private GameObject go_ScrollViewContent;

	[SerializeField]
	private GameObject go_FoundBeacon;
	List<GameObject> go_FoundBeaconCloneList = new List<GameObject>();
	GameObject go_FoundBeaconClone;
	private float f_ScrollViewContentRectWidth;
	private float f_ScrollViewContentRectHeight;
	private int i_BeaconCounter = 0;

	// Receive
	private List<Beacon> mybeacons = new List<Beacon>();

	private void Start() {
		setBeaconPropertiesAtStart(); // please keep here!

		_bluetoothButton.onClick.AddListener(delegate() {
			BluetoothState.EnableBluetooth();
		});
		_bluetoothText = _bluetoothButton.GetComponentInChildren<Text>();
		BluetoothState.BluetoothStateChangedEvent += delegate(BluetoothLowEnergyState state) {
			switch (state) {
			case BluetoothLowEnergyState.TURNING_OFF:
			case BluetoothLowEnergyState.TURNING_ON:
				break;
			case BluetoothLowEnergyState.UNKNOWN:
			case BluetoothLowEnergyState.RESETTING:
				SwitchToStatus();
				_statusText.text = "Checking Device…";
				break;
			case BluetoothLowEnergyState.UNAUTHORIZED:
				SwitchToStatus();
				_statusText.text = "You don't have the permission to use beacons.";
				break;
			case BluetoothLowEnergyState.UNSUPPORTED:
				SwitchToStatus();
				_statusText.text = "Your device doesn't support beacons.";
				break;
			case BluetoothLowEnergyState.POWERED_OFF:
				SwitchToMenu();
				_bluetoothButton.interactable = true;
				_bluetoothText.text = "Enable Bluetooth";
				break;
			case BluetoothLowEnergyState.POWERED_ON:
				SwitchToMenu();
				_bluetoothButton.interactable = false;
				_bluetoothText.text = "Bluetooth already enabled";
				break;
			case BluetoothLowEnergyState.IBEACON_ONLY:
				SwitchToMenu();
				_bluetoothButton.interactable = false;
				_bluetoothText.text = "iBeacon only";
				break;
			default:
				SwitchToStatus();
				_statusText.text = "Unknown Error";
				break;
			}
		};
		f_ScrollViewContentRectWidth = ((RectTransform)go_FoundBeacon.transform).rect.width;
		f_ScrollViewContentRectHeight = ((RectTransform)go_FoundBeacon.transform).rect.height;
		BluetoothState.Init();
		Input.location.Start();
	}

	private void SwitchToStatus() {
		_statusScreen.SetActive(true);
		_menuScreen.SetActive(false);
	}

	private void SwitchToMenu() {
		_statusScreen.SetActive(false);
		_menuScreen.SetActive(true);
	}

	private void setBeaconPropertiesAtStart() {
		RestorePlayerPrefs();
		if (bm_Mode == BroadcastMode.unknown) { // first start
			bm_Mode = BroadcastMode.receive;
			bt_Type = BeaconType.iBeacon;
			if (iBeaconServer.region.regionName != "") {
				Debug.Log("check iBeaconServer-inspector");
				s_Region = iBeaconServer.region.regionName;
				bt_Type 	= iBeaconServer.region.beacon.type;
				if (bt_Type == BeaconType.EddystoneURL) {
					s_UUID = iBeaconServer.region.beacon.UUID;
				} else if (bt_Type == BeaconType.EddystoneUID) {
					s_UUID = iBeaconServer.region.beacon.UUID;
					s_Major = iBeaconServer.region.beacon.instance;
				} else if (bt_Type == BeaconType.iBeacon) {
					s_UUID = iBeaconServer.region.beacon.UUID;
					s_Major = iBeaconServer.region.beacon.major.ToString();
					s_Minor = iBeaconServer.region.beacon.minor.ToString();
				}
			} else if (iBeaconReceiver.regions.Length != 0) {
				Debug.Log("check iBeaconReceiver-inspector");
				s_Region	= iBeaconReceiver.regions[0].regionName;
				bt_Type 	= iBeaconReceiver.regions[0].beacon.type;
				if (bt_Type == BeaconType.EddystoneURL) {
					s_UUID = iBeaconReceiver.regions[0].beacon.UUID;
				} else if (bt_Type == BeaconType.EddystoneUID) {
					s_UUID = iBeaconReceiver.regions[0].beacon.UUID;
					s_Major = iBeaconReceiver.regions[0].beacon.instance;
				} else if (bt_Type == BeaconType.iBeacon) {
					s_UUID = iBeaconReceiver.regions[0].beacon.UUID;
					s_Major = iBeaconReceiver.regions[0].beacon.major.ToString();
					s_Minor = iBeaconReceiver.regions[0].beacon.minor.ToString();
				} 
			}
		}
		InputDropdown.value = (int)bt_Type;
		bs_State = BroadcastState.inactive;
		SetBeaconProperties();
		SetBroadcastMode();
		SetBroadcastState();
		Debug.Log("Beacon properties and modes restored");
	}

	// Beacon Properties
	public void btn_changeUUID() { // onRelease
		// Beaconregion
		if (input_Region.text != null && input_Region.text != "")
			s_Region	= input_Region.text;
		// Beacontype
		bt_Type = bt_PendingType;
		// UUID, Namespace, Url
		if (input_UUID.text != null && input_UUID.text != "")
			s_UUID		= input_UUID.text;
		// Major or Instance on send
		if (input_Major .text != null && input_Major.text != "")
			s_Major		= input_Major.text;
		// Minor on send
		if (input_Minor .text != null && input_Minor.text != "")
			s_Minor		= input_Minor.text;
		input_Region.image.color = Color.white;
		input_Region.text = "";
		input_UUID.image.color	 = Color.white;
		input_UUID.text = "";
		input_Major.image.color	 = Color.white;
		input_Major.text = "";
		input_Minor.image.color	 = Color.white;
		input_Minor.text = "";
		InputDropdown.image.color = Color.white;
		SetBeaconProperties();
		SavePlayerPrefs();
	}
	public void dd_onBeaconTypeChanged(Dropdown target) {
		bt_PendingType = (BeaconType)target.value;
		if (bt_PendingType == BeaconType.iBeacon) {
			set_iBeaconText();
		} else if (bt_PendingType == BeaconType.EddystoneUID) {
			set_EddyUIDText();
		} else if (bt_PendingType == BeaconType.EddystoneURL) {
			set_EddyUrlText();
		} else { // any
			set_AnyText();
		}
	}
	private void set_iBeaconText() {
		input_UUID.gameObject.SetActive(true);
		txt_inputUUIDChar.text		= "Enter UUID...";
		input_Major.gameObject.SetActive(true);
		input_Major.contentType = InputField.ContentType.IntegerNumber;
		txt_inputMajorChar.text		= "Enter Major...";
		input_Minor.gameObject.SetActive(true);
		txt_inputMinorChar.text		= "Enter Minor...";
	}
	private void set_EddyUIDText() {
		input_UUID.gameObject.SetActive(true);
		txt_inputUUIDChar.text		= "Enter Namespace...";
		input_Major.gameObject.SetActive(true);
		input_Major.contentType = InputField.ContentType.Standard;
		txt_inputMajorChar.text		= "Enter Instance...";
		input_Minor.gameObject.SetActive(false);
	}
	private void set_EddyUrlText() {
		input_UUID.gameObject.SetActive(true);
		txt_inputUUIDChar.text		= "Enter Url...";
		input_Major.gameObject.SetActive(false);
		input_Minor.gameObject.SetActive(false);
	}
	private void set_AnyText() {
		input_UUID.gameObject.SetActive(false);
		input_Major.gameObject.SetActive(false);
		input_Minor.gameObject.SetActive(false);
	}

	private void SetBeaconProperties() {
		// setText
		txt_actualType.text		= bt_Type.ToString();
		txt_actualRegion.text	= s_Region;
		if (bt_Type == BeaconType.iBeacon) {
			txt_actualUUIDChar.gameObject.SetActive(true);
			txt_actualUUIDChar.text		= "actual UUID:";
			txt_actualUUID.gameObject.SetActive(true);
			txt_actualUUID.text			= s_UUID;
			txt_actualMajorChar.gameObject.SetActive(true);
			txt_actualMajorChar.text	= "actual Major:";
			txt_actualMajor.gameObject.SetActive(true);
			txt_actualMajor.text		= s_Major;
			txt_actualMinorChar.gameObject.SetActive(true);
			txt_actualMinor.gameObject.SetActive(true);
			txt_actualMinor.text		= s_Minor;
			set_iBeaconText();
		} else if (bt_Type == BeaconType.EddystoneUID) {
			txt_actualUUIDChar.gameObject.SetActive(true);
			txt_actualUUIDChar.text		= "actual Namespace:";
			txt_actualUUID.gameObject.SetActive(true);
			txt_actualUUID.text			= s_UUID;
			txt_actualMajorChar.gameObject.SetActive(true);
			txt_actualMajorChar.text	= "actual Instance";
			txt_actualMajor.gameObject.SetActive(true);
			txt_actualMajor.text		= s_Major;
			txt_actualMinorChar.gameObject.SetActive(false);
			txt_actualMinor.gameObject.SetActive(false);
			set_EddyUIDText();
		} else if (bt_Type == BeaconType.EddystoneURL) {
			txt_actualUUIDChar.gameObject.SetActive(true);
			txt_actualUUIDChar.text		= "actual Url:";
			txt_actualUUID.gameObject.SetActive(true);
			txt_actualUUID.text			= s_UUID;
			txt_actualMajorChar.gameObject.SetActive(false);
			txt_actualMajor.gameObject.SetActive(false);
			txt_actualMinorChar.gameObject.SetActive(false);
			txt_actualMinor.gameObject.SetActive(false);
			set_EddyUrlText();
		} else { // any
			txt_actualUUIDChar.gameObject.SetActive(false);
			txt_actualUUID.gameObject.SetActive(false);
			txt_actualMajorChar.gameObject.SetActive(false);
			txt_actualMajor.gameObject.SetActive(false);
			txt_actualMinorChar.gameObject.SetActive(false);
			txt_actualMinor.gameObject.SetActive(false);
			set_AnyText();
		}
	}

	// BroadcastMode
	public void btn_switch() { 
		//Debug.Log("Button Switch pressed");
		if (bm_Mode == BroadcastMode.receive)
			bm_Mode = BroadcastMode.send;
		else
			bm_Mode = BroadcastMode.receive;
		SetBroadcastMode();
		SavePlayerPrefs();
	}
	private void SetBroadcastMode() { 
		// setText
		txt_BroadcastMode_LabelText.text = bm_Mode.ToString();
		if (bm_Mode == BroadcastMode.receive)
			txt_BroadcastMode_ButtonText.text = "Switch to " + BroadcastMode.send.ToString();
		else
			txt_BroadcastMode_ButtonText.text = "Switch to " + BroadcastMode.receive.ToString();
	}

	// BroadcastState
	public void btn_StartStop() {
		//Debug.Log("Button Start / Stop pressed");
		/*** Beacon will start ***/
		if (bs_State == BroadcastState.inactive) {
			// ReceiveMode
			if (bm_Mode == BroadcastMode.receive) {
				iBeaconReceiver.BeaconRangeChangedEvent += OnBeaconRangeChanged;
				// check if all mandatory propertis are filled
				if (s_Region == null || s_Region == "") {
					input_Region.image.color= Color.red;
					return;
				}
				if (bt_Type == BeaconType.Any) {
					iBeaconReceiver.regions = new iBeaconRegion[]{new iBeaconRegion(s_Region, new Beacon())};
				} else if (bt_Type == BeaconType.EddystoneEID) {
					iBeaconReceiver.regions = new iBeaconRegion[]{new iBeaconRegion(s_Region, new Beacon(BeaconType.EddystoneEID))};
				} else {
					if (s_UUID == null || s_UUID == "") {
						input_UUID.image.color= Color.red;
						return;
					}
					if (bt_Type == BeaconType.iBeacon) {
						iBeaconReceiver.regions = new iBeaconRegion[]{new iBeaconRegion(s_Region, new Beacon(s_UUID, Convert.ToInt32(s_Major), Convert.ToInt32(s_Minor)))};
					} else if (bt_Type == BeaconType.EddystoneUID) {
						iBeaconReceiver.regions = new iBeaconRegion[]{new iBeaconRegion(s_Region, new Beacon(s_UUID, "")) };
					} else if (bt_Type == BeaconType.EddystoneURL) {
						iBeaconReceiver.regions = new iBeaconRegion[]{new iBeaconRegion(s_Region, new Beacon(s_UUID))};
					}
				}
				// !!! Bluetooth has to be turned on !!! TODO
				iBeaconReceiver.Scan();
				Debug.Log ("Listening for beacons");
			}
			// SendMode
			else {
				// check if all mandatory propertis are filled
				if (s_Region == null || s_Region == "") {
					input_Region.image.color= Color.red;
					return;
				}
				if (bt_Type == BeaconType.EddystoneEID) {
					InputDropdown.image.color = Color.red;
				}
				if (bt_Type == BeaconType.Any) {
					iBeaconServer.region = new iBeaconRegion(s_Region, new Beacon());
				} else {
					if (s_UUID == null || s_UUID == "") {
						input_UUID.image.color= Color.red;
						return;
					}
					if (bt_Type == BeaconType.EddystoneURL) {
						iBeaconServer.region = new iBeaconRegion(s_Region, new Beacon(s_UUID));
					} else {
						if (s_Major == null || s_Major == "") {
							input_Major.image.color= Color.red;
							return;
						}
						if (bt_Type == BeaconType.EddystoneUID) {
							iBeaconServer.region = new iBeaconRegion(s_Region, new Beacon(s_UUID, s_Major));
						} else if (bt_Type == BeaconType.iBeacon) {
							if (s_Minor == null || s_Minor == "") {
								input_Major.image.color= Color.red;
								return;
							}
							iBeaconServer.region = new iBeaconRegion(s_Region, new Beacon(s_UUID, Convert.ToInt32(s_Major), Convert.ToInt32(s_Minor)));
						}
					}
				}
				// !!! Bluetooth has to be turned on !!! TODO
				iBeaconServer.Transmit();
				Debug.Log ("It is on, go sending");
			}
			bs_State = BroadcastState.active;
			img_ButtonBroadcastState.color = Color.red;
		} else {
			if (bm_Mode == BroadcastMode.receive) {// Stop for receive
				iBeaconReceiver.Stop();
				iBeaconReceiver.BeaconRangeChangedEvent -= OnBeaconRangeChanged;
				removeFoundBeacons();
			} else { // Stop for send
				iBeaconServer.StopTransmit();
			}
			bs_State = BroadcastState.inactive;
			img_ButtonBroadcastState.color = Color.green;
		}
		SetBroadcastState();
		SavePlayerPrefs();
	}
	private void SetBroadcastState() {
		// setText
		if (bs_State == BroadcastState.inactive)
			txt_BroadcastState_ButtonText.text = "Start";
		else
			txt_BroadcastState_ButtonText.text = "Stop";
		txt_BroadcastState_LabelText.text = bs_State.ToString();
	}

	private void OnLocationUpdate()
    {
		Input.location.Start(0.1f, 0.1f);

		// Wait until service initializes
		var started = DateTime.Now;
		var isPastMax = false;
		while (Input.location.status == LocationServiceStatus.Initializing && isPastMax)
		{
			var current = DateTime.Now;
			if(started.Subtract(current).TotalSeconds > 5)
            {
				isPastMax = true;
            }
		}

		// Service didn't initialize in 20 seconds
		if (isPastMax || Input.location.status == LocationServiceStatus.Failed)
		{
			txt_location.text = "failed";
			return;
		}

		// Access granted and location value could be retrieved
		txt_location.text = "Lat: " + Input.location.lastData.latitude + " Long: " + Input.location.lastData.longitude;

	}

	public double ToRadians(double degrees)
	{
		return (PI / 180) * degrees;
	}

	public double ToDegrees(double radians)
	{
		return (180 / PI) * radians;
	}


	private string Str(double[] a)
    {
		return string.Join(", ", a);
    }

	// https://gis.stackexchange.com/questions/66/trilateration-using-3-latitude-longitude-points-and-3-distances
	private Tuple<double, double> Trilateration(double DistA, double DistB, double DistC)
    {
		var earthR = 6371;
		// A
		var LatA = 32.813129;
		var LonA = -96.795350;
		// B
		var LatB = 32.813083;
		var LonB = -96.795375;
		// C
		var LatC = 32.813089;
		var LonC = -96.795333;
		// using authalic sphere
		// Convert geodetic Lat/Long to ECEF xyz
		//    1. Convert Lat/Long to radians
		//    2. Convert Lat/Long(radians) to ECEF

		var xA = earthR * (Cos(ToRadians(LatA)) * Cos(ToRadians(LonA)));
		var yA = earthR * (Cos(ToRadians(LatA)) * Sin(ToRadians(LonA)));
		var zA = earthR * Sin(ToRadians(LatA));

		var xB = earthR * (Cos(ToRadians(LatB)) * Cos(ToRadians(LonB)));
		var yB = earthR * (Cos(ToRadians(LatB)) * Sin(ToRadians(LonB)));
		var zB = earthR * Sin(ToRadians(LatB));

		var xC = earthR * (Cos(ToRadians(LatC)) * Cos(ToRadians(LonC)));
		var yC = earthR * (Cos(ToRadians(LatC)) * Sin(ToRadians(LonC)));
		var zC = earthR * Sin(ToRadians(LatC));

		var p1 = new double[] { xA, yA, zA };
		var p2 = new double[] { xB, yB, zB };
		var p3 = new double[] { xC, yC, zC };
		Debug.Log($"Sean - p1: ${Str(p1)}");
		Debug.Log($"Sean - p2: ${Str(p2)}");
		Debug.Log($"Sean - p3: ${Str(p3)}");

		var ex = Div(Diff(p2, p1), Magnitude(Diff(p2, p1)));
		Debug.Log($"Sean - ex: ${Str(ex)}");

		var i = Dot(ex, Diff(p3, p1));
		var diff = Diff(Diff(p3, p1), Mul(ex, i));
		var ey = Normalize(diff);
		Debug.Log($"Sean - ey: ${Str(ey)}");
		var ez = Cross(ex, ey);
		Debug.Log($"Sean - ez: ${Str(ez)}");
		var d = Magnitude(Diff(p2, p1));
		var j = Dot(ey, Diff(p3, p1));

		var x = (Pow(DistA, 2) - Pow(DistB, 2) + Pow(d, 2)) / (2 * d);
		var y = ((Pow(DistA, 2) - Pow(DistC, 2) + Pow(i, 2) + Pow(j, 2)) / (2 * j)) - ((i / j) * x);
		Debug.Log($"Sean - x: ${x}");
		Debug.Log($"Sean - y: ${y}");

		var z = Sqrt(Pow(DistA, 2) - Pow(x, 2) - Pow(y, 2));
		Debug.Log($"Sean - z: ${z}");
		var triPt = Add(Add(p1, Mul(ex, x)), Add(Mul(ey, y), Mul(ez, z)));
		Debug.Log($"Sean - triPt: ${Str(triPt)}");
		var lat =  ToDegrees(Asin(triPt[2] / earthR));
		var lon = ToDegrees(Atan2(triPt[1], triPt[0]));
		return Tuple.Create(lat, lon);
	}

	private void OnBeaconRangeChanged(Beacon[] beacons) {
		var distances = new double[] { 0, 0, 0 };
		foreach (Beacon b in beacons) {
			var index = mybeacons.IndexOf(b);
			if (index == -1) {
				mybeacons.Add(b);
			} else {
				mybeacons[index] = b;
			}
			var minor = mybeacons[index].minor;
			distances[minor] = mybeacons[index].accuracy;
		}
		try
        {

			var calcLocation = Trilateration(distances[0], distances[1], distances[2]);
			Debug.Log($"Sean - Calculated location: ${calcLocation.Item1}, ${calcLocation.Item2}");

			// calculate lat/long
			txt_calcLatLong.text = "Lat: " + calcLocation.Item1 + " Long: " + calcLocation.Item2;
		} catch(Exception e)
        {
			Debug.Log($"Sean - e: ${e}");
			Debug.Log($"Sean - stack: ${e.StackTrace}");
		}
		for (int i = mybeacons.Count - 1; i >= 0; --i) {
			if (mybeacons[i].lastSeen.AddSeconds(10) < DateTime.Now) {
				mybeacons.RemoveAt(i);
			}
		}
		OnLocationUpdate();
		DisplayOnBeaconFound();
	}

	private void DisplayOnBeaconFound() {
		removeFoundBeacons();
		RectTransform rt_Content = (RectTransform)go_ScrollViewContent.transform;
		foreach (Beacon b in mybeacons) {
			// create clone of foundBeacons
			go_FoundBeaconClone = Instantiate(go_FoundBeacon);
			// make it child of the ScrollView
			go_FoundBeaconClone.transform.SetParent(go_ScrollViewContent.transform);
			// get resolution based scalefactor
			float f_scaler = ((RectTransform)go_FoundBeaconClone.transform).localScale.y;
			Vector2 v2_scale = new Vector2(1,1);
			// reset scalefactor
			go_FoundBeaconClone.transform.localScale = v2_scale;
			// get anchorposition
			Vector3 pos = go_ScrollViewContent.transform.position; 
			// positioning
			pos.y -= f_ScrollViewContentRectHeight/f_scaler * i_BeaconCounter;
			go_FoundBeaconClone.transform.position = pos;
			i_BeaconCounter++;
			// resize scrollviewcontent
			rt_Content.sizeDelta = new Vector2(f_ScrollViewContentRectWidth,f_ScrollViewContentRectHeight*i_BeaconCounter);
			go_FoundBeaconClone.SetActive(true);
			// adding reference to instance
			go_FoundBeaconCloneList.Add(go_FoundBeaconClone);
			// get textcomponents
			Text[]Texts	= go_FoundBeaconClone.GetComponentsInChildren<Text>();
			// deleting placeholder
			foreach (Text t in Texts)
				t.text = "";
			Debug.Log ("fond Beacon: " + b.ToString());
			switch (b.type) {
			case BeaconType.iBeacon:
				Texts[0].text 	= "UUID:";
				Texts[1].text 	= b.UUID.ToString();
				Texts[2].text 	= "Major:";
				Texts[3].text	= b.major.ToString();
				Texts[4].text 	= "Minor:";
				Texts[5].text	= b.minor.ToString();
				Texts[6].text 	= "Range:";
				Texts[7].text	= b.range.ToString();
				Texts[8].text 	= "Strength:";
				Texts[9].text	= b.strength.ToString() + " db";
				Texts[10].text 	= "Accuracy:";
				Texts[11].text	= b.accuracy.ToString().Substring(0,10) + " m";
				Texts[12].text 	= "Rssi:";
				Texts[13].text	= b.rssi.ToString() + " db";
				break;
			case BeaconType.EddystoneUID:
				Texts[0].text 	= "Namespace:";
				Texts[1].text 	= b.UUID;
				Texts[2].text 	= "Instance:";
				Texts[3].text	= b.instance;
				Texts[6].text 	= "Range:";
				Texts[7].text	= b.range.ToString();
				break;
			case BeaconType.EddystoneURL:
				Texts[0].text 	= "URL:";
				Texts[1].text 	= b.UUID.ToString();
				Texts[2].text 	= "Range:";
				Texts[3].text	= b.range.ToString();
				break;
			case BeaconType.EddystoneEID:
				Texts[0].text 	= "EID:";
				Texts[1].text 	= b.UUID.ToString();
				Texts[2].text 	= "Range:";
				Texts[3].text	= b.range.ToString();
				break;
			default:
				break;
			}
		}
	}

	private void removeFoundBeacons() {
		Debug.Log("removing all found Beacons");
		// set scrollviewcontent to standardsize
		RectTransform rt_Content = (RectTransform)go_ScrollViewContent.transform;
		rt_Content.sizeDelta = new Vector2(f_ScrollViewContentRectWidth,f_ScrollViewContentRectHeight);
		// destroying each clone
		foreach (GameObject go in go_FoundBeaconCloneList)
			Destroy(go);
		go_FoundBeaconCloneList.Clear();
		i_BeaconCounter = 0;
	}

	// PlayerPrefs
	private void SavePlayerPrefs() {
		PlayerPrefs.SetInt("Type", (int)bt_Type);
		PlayerPrefs.SetString("Region", s_Region);
		PlayerPrefs.SetString("UUID", s_UUID);
		PlayerPrefs.SetString("Major", s_Major);
		PlayerPrefs.SetString("Minor", s_Minor);
		PlayerPrefs.SetInt("BroadcastMode", (int)bm_Mode);
		//PlayerPrefs.DeleteAll();
	}
	private void RestorePlayerPrefs() {
		if (PlayerPrefs.HasKey("Type"))
			bt_Type = (BeaconType)PlayerPrefs.GetInt("Type");
		if (PlayerPrefs.HasKey("Region"))
			s_Region = PlayerPrefs.GetString("Region");
		if (PlayerPrefs.HasKey("UUID"))
			s_UUID = PlayerPrefs.GetString("UUID");
		if (PlayerPrefs.HasKey("Major"))
			s_Major = PlayerPrefs.GetString("Major");
		if (PlayerPrefs.HasKey("Minor"))
			s_Minor = PlayerPrefs.GetString("Minor");
		if (PlayerPrefs.HasKey("BroadcastMode"))
			bm_Mode = (BroadcastMode)PlayerPrefs.GetInt("BroadcastMode");
		else 
			bm_Mode = BroadcastMode.unknown;
	}
}