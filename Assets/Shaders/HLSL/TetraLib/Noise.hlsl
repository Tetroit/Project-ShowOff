#ifndef TETRALIB_NOISE_INCLUDED
#define TETRALIB_NOISE_INCLUDED

float nrand(float uv)
{
    return frac(sin(dot(uv, 12.9898)) * 43758.5453);
}
float nrand(float2 uv)
{
    return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
}
float nrand(float3 uv)
{
    return frac(sin(dot(uv, float3(12.9898, 78.233, 975.7463))) * 43758.5453);
}
float nrand(float4 uv)
{
    return frac(sin(dot(uv, float4(12.9898, 78.233, 975.7463, 47.9264))) * 43758.5453);
}
float2 nrand2(float2 uv)
{
    return float2(
        nrand(uv * 5.2486 + float2(-97.5927, 1825.97)),
        nrand(uv * 4.486 + float2(92.47, -25.97))
   );
}
float3 nrand3(float3 uv)
{
    return float3(
        nrand(uv * 195.2486 + float3(-97.5927, 1825.97, 853.297)),
        nrand(uv * 4.486 + float3(92.47, -25.97, 18.533)),
        nrand(uv * 12.5686 + float3(4.61, 95.172, -511.297))
   );
}
float4 nrand4(float3 uv)
{
    return float4(
        nrand(uv * float3(-0.5927, 0.97, 0.85297)),
        nrand(uv * float3(0.47, -0.27, 0.533)),
        nrand(uv * float3(0.61, 0.172, -0.297)),
        nrand(uv * float3(0.61, -1.2172, -0.297))
   );
}

float LerpNoise(float fac)
{
    float peak1 = nrand(floor(fac));
    float peak2 = nrand(ceil(fac));
    float peakFac = frac(fac);
    //return lerp(peak1, peak2, peakFac);
    return lerp(peak1, peak2, smoothstep(0,1,peakFac));
}
float LerpNoise(float2 fac)
{
    float peak11 = nrand(floor(fac));
    float peak22 = nrand(ceil(fac));
    float peak12 = nrand(float2(floor(fac.x), ceil(fac.y)));
    float peak21 = nrand(float2(ceil(fac.x), floor(fac.y)));
    
    float2 peakFac = smoothstep(0,1,frac(fac));

    float peak1 = lerp(peak11, peak12, peakFac.y);
    float peak2 = lerp(peak21, peak22, peakFac.y);

    return lerp(peak1, peak2, peakFac.x);
}
float Voronoi3D (float3 pos, float3 scale, out float3 nodePos)
{
	pos /= scale;
    float3 i = floor(pos);

    float minDist = 10;
    float minFac = 0;

    for (int x = -1; x <= 1; x++)
    {
        for (int y = -1; y <= 1; y++)
        {
            for (int z = -1; z <= 1; z++)
            {
                float3 g = float3(x, y, z) + i;
                float4 rg = nrand4(g);
                float3 node =  rg.xyz - float3(0.5, 0.5, 0.5) + g;
                float dist = distance(pos,node);
                if (dist < minDist)
                {
                    minDist = dist;
                    nodePos = node * scale;
                    minFac = rg.w;
                }
            }
        }
    }
    return minFac;
}

//NOT IMPLEMENTED YET
float Perlin3D (float3 pos, float3 scale)
{
    
}

#endif 
//TETRALIB_NOISE_INCLUDED