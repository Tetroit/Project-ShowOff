#ifndef TETRALIB_NOISE_INCLUDED
#define TETRALIB_NOISE_INCLUDED

#include "./Math.hlsl"

#ifndef PI
#define PI 3.141593
#endif

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
        nrand(uv * 195.2486 + float3(-97.5927, 182.97, 83.297)),
        nrand(uv * 4.486 + float3(92.47, -25.97, 18.533))
   );
}
float3 nrand3(float2 uv)
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

float2 grad2(float2 seed)
{
    float2 raw = nrand2(seed);
    float s = sin(raw.y * 2 * PI);
    float c = cos(raw.y * 2 * PI);
    float2 res = float2(raw.x * c, raw.x * s);
    return res;
}

void Voronoi3D_float (float3 pos, float3 scale, out float3 nodePos, out float fac)
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
    fac = minFac;
}
void Voronoi2D_float (float2 pos, float2 scale, out float2 nodePos, out float fac)
{
	pos /= scale;
    float2 i = floor(pos);

    float minDist = 10;
    float minFac = 0;

    for (int x = -1; x <= 1; x++)
    {
        for (int y = -1; y <= 1; y++)
        {
            float2 g = float2(x, y) + i;
            float3 rg = nrand3(g);
            float2 node =  rg.xy - float2(0.5f, 0.5f) + g;
            float dist = distance(pos,node);
            if (dist < minDist)
            {
                minDist = dist;
                nodePos = node * scale;
                minFac = rg.z;
            }
        }
    }
    fac = minFac;
}

void Perlin2D_float (float2 pos, float2 scale, out float2 dir, out float fac)
{
    pos *= scale;

    float2 fraction;
    float2 whole;
    fraction = modf(pos, whole);
    float2 corners[4] = 
    {
        grad2(whole + float2(0,0)),
        grad2(whole + float2(1,0)),
        grad2(whole + float2(0,1)),
        grad2(whole + float2(1,1))
    };

    float2 facMin = fraction;
    float2 facMax = float2(1,1) - facMin;
    
    float2 toCorners[4] =
    {
        float2(facMin.x, facMin.y),
        float2(-facMax.x, facMin.y),
        float2(facMin.x, -facMax.y),
        float2(-facMax.x, -facMax.y)
    };
    float dots[4];
    
    for (int i=0; i<4; i++)
    {
        dots[i] = dot(corners[i],toCorners[i]);
    }

    dir = 0;
    for (int i=0; i<4; i++)
    {
        dir += abs(toCorners[i].x) * abs(toCorners[i].y) * corners[i];
    }

    float nx1 = lerp (dots[0], dots[1], smoothLerp(fraction.x));
    float nx2 = lerp (dots[2], dots[3], smoothLerp(fraction.x));
    fac = lerp (nx1, nx2, smoothLerp(fraction.y));
    
    float2 dx1 = lerp (corners[0], corners[1], smoothLerp(fraction.x));
    float2 dx2 = lerp (corners[2], corners[3], smoothLerp(fraction.x));
    dir = lerp (dx1, dx2, smoothLerp(fraction.y));
}

//NOT IMPLEMENTED YET
void Perlin3D_float (float3 pos, float3 scale)
{
    
}

#endif 
//TETRALIB_NOISE_INCLUDED