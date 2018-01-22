x = linspace(0, 1);
y1 = ChilliantGamma(x);
y2 = TaylorGamma(x);
y3 = ApproxGamma(x);

figure
plot(x, y1, x, y2, x, y3);

function g = ChilliantGamma(x)
    g = 1.055 * power(x, (1.0/2.4)) - 0.055;
end

function g = TaylorGamma(x)
    g = x .* (-0.85373472095 .* x + 1.79284291400159);
end

function g = ApproxGamma(x)
    g = x .* (-0.96 .* x + 1.92);
end