<?xml version="1.0" encoding="UTF-8"?>
<MiddleVR>
	<Kernel LogLevel="2" LogInSimulationFolder="0" EnableCrashHandler="0" Version="1.7.0.5" />
	<DeviceManager>
		<Driver Type="vrDriverDirectInput" />
		<Driver Type="vrDriverRazerHydra" />
		<Wand Name="Wand0" Driver="0" Axis="Mouse.Axis" HorizontalAxis="0" HorizontalAxisScale="1" VerticalAxis="1" VerticalAxisScale="-1" AxisDeadZone="0.3" Buttons="RazerHydra.Joystick0.Buttons" Button0="0" Button1="1" Button2="2" Button3="3" Button4="4" Button5="5" />
	</DeviceManager>
	<DisplayManager Fullscreen="0" AlwaysOnTop="1" WindowBorders="0" ShowMouseCursor="0" VSync="1" GraphicsRenderer="1" AntiAliasing="0" ForceHideTaskbar="0" SaveRenderTarget="0" ChangeWorldScale="0" WorldScale="1">
		<Node3D Name="VRSystemCenterNode" Tag="VRSystemCenter" Parent="None" Tracker="0" IsFiltered="0" Filter="0" PositionLocal="0.000000,0.000000,0.000000" OrientationLocal="0.000000,0.000000,0.000000,1.000000" />
		<Node3D Name="CenterNode" Parent="VRSystemCenterNode" Tracker="0" IsFiltered="0" Filter="0" PositionLocal="0.000000,0.000000,0.000000" OrientationLocal="0.000000,0.000000,0.000000,1.000000" />
		<Node3D Name="HandOffset" Parent="CenterNode" Tracker="0" IsFiltered="0" Filter="0" PositionLocal="0.000000,0.300000,1.300000" OrientationLocal="0.000000,0.000000,0.000000,1.000000" />
		<Node3D Name="HandNode" Tag="Hand" Parent="HandOffset" Tracker="RazerHydra.Tracker0" IsFiltered="0" Filter="0" UseTrackerX="1" UseTrackerY="1" UseTrackerZ="1" UseTrackerYaw="1" UseTrackerPitch="1" UseTrackerRoll="1" />
		<Node3D Name="LHAND" Parent="HandOffset" Tracker="0" IsFiltered="0" Filter="0" PositionLocal="-0.300000,0.000000,0.000000" OrientationLocal="0.270598,0.270598,0.653282,0.653281" />
		<Node3D Name="RHAND" Parent="HandOffset" Tracker="0" IsFiltered="0" Filter="0" PositionLocal="0.300000,0.000000,0.000000" OrientationLocal="0.430232,0.206082,0.256472,0.840626" />
		<Node3D Name="NeckNode" Parent="CenterNode" Tracker="0" IsFiltered="0" Filter="0" PositionLocal="0.000000,0.000000,2.000000" OrientationLocal="0.000000,0.000000,0.000000,1.000000" />
		<Node3D Name="HeadNode" Parent="NeckNode" Tracker="RazerHydra.Tracker0" IsFiltered="0" Filter="0" UseTrackerX="1" UseTrackerY="1" UseTrackerZ="1" UseTrackerYaw="1" UseTrackerPitch="1" UseTrackerRoll="1" />
		<Camera Name="Camera0" Parent="HeadNode" Tracker="0" IsFiltered="0" Filter="0" PositionLocal="0.000000,0.000000,0.000000" OrientationLocal="0.000000,0.000000,0.000000,1.000000" VerticalFOV="60" Near="0.1" Far="5000" Screen="0" ScreenDistance="1" UseViewportAspectRatio="1" AspectRatio="1.33333" />
		<Node3D Name="LFOOT" Parent="CenterNode" Tracker="0" IsFiltered="0" Filter="0" PositionLocal="-0.200000,0.000000,0.050000" OrientationLocal="0.000000,0.000000,0.000000,1.000000" />
		<Node3D Name="RFOOT" Parent="CenterNode" Tracker="0" IsFiltered="0" Filter="0" PositionLocal="0.200000,0.000000,0.050000" OrientationLocal="0.000000,0.000000,0.000000,1.000000" />
		<Node3D Name="HipsOffset" Parent="CenterNode" Tracker="0" IsFiltered="0" Filter="0" PositionLocal="0.000000,0.000000,1.100000" OrientationLocal="0.000000,0.000000,0.000000,1.000000" />
		<Node3D Name="HIPS" Parent="HipsOffset" Tracker="0" IsFiltered="0" Filter="0" PositionLocal="0.100000,0.100000,0.100000" OrientationLocal="0.000000,0.000000,0.000000,1.000000" />
		<Viewport Name="Viewport0" Left="0" Top="0" Width="1280" Height="800" Camera="Camera0" Stereo="0" StereoMode="3" CompressSideBySide="0" StereoInvertEyes="0" OculusRiftWarping="1" OffScreen="0" UseHomography="0" />
	</DisplayManager>
	<ClusterManager NVidiaSwapLock="0" DisableVSyncOnServer="1" ForceOpenGLConversion="0" BigBarrier="0" SimulateClusterLag="0" MultiGPUEnabled="0" ImageDistributionMaxPacketSize="8000" ClientConnectionTimeout="60" />
</MiddleVR>
