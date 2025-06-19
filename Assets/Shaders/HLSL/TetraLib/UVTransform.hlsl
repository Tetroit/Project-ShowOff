#ifndef UV_TRANSFORM_INCLUDED
#define UV_TRANSFORM_INCLUDED


#ifndef UNITY_HDRP
struct UVInputs
{
	float2 uvNormalized;
	float2 uvScreen;
	float2 uvScreenC;
	float2 uvRatioMin;
	float2 uvRatioMax;
	float2 uvRatioMinC;
	float2 uvRatioMaxC;
};

UVInputs GetUVInputs(float2 uvNormalized)
{
	UVInputs inputs;
	inputs.uvNormalized = uvNormalized;
	inputs.uvScreen = uvNormalized * _ScreenParams.xy;
	inputs.uvScreenC = inputs.uvScreen - _ScreenParams.xy * 0.5;

	if (_ScreenParams.w < _ScreenParams.z)
	{
		inputs.uvRatioMax = inputs.uvScreen * (_ScreenParams.ww - 1);
		inputs.uvRatioMin = inputs.uvScreen * (_ScreenParams.zz - 1);
		inputs.uvRatioMaxC = inputs.uvScreenC * (_ScreenParams.ww - 1);
		inputs.uvRatioMinC = inputs.uvScreenC * (_ScreenParams.zz - 1);
	}
	else
	{
		inputs.uvRatioMax = inputs.uvScreen * (_ScreenParams.zz - 1);
		inputs.uvRatioMin = inputs.uvScreen * (_ScreenParams.ww - 1);
		inputs.uvRatioMaxC = inputs.uvScreenC * (_ScreenParams.zz - 1);
		inputs.uvRatioMinC = inputs.uvScreenC * (_ScreenParams.ww - 1);
	}
	return inputs;
}
#endif //HPRP


#endif