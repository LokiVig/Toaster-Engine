float4 VS(float4 Pos : POSITION) : SV_POSITION {
    return Pos;
}

float4 main(float4 Pos : SV_POSITION) : SV_Target {
    return float4(1, 0, 0, 1); // Red color
}
