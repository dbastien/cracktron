﻿using System;

public class Histogram
{
    public int[] Bins;
    public double[] BinTotals;

    public float BinMin;
    public float BinMax;
    public float BinWidth;

    public Histogram(float min, float max, int numBins)
    {
        if (numBins <= 0 || max <= min)
        {
            throw new ArgumentOutOfRangeException();
        }

        BinMin = min;
        BinMax = max;

        Bins = new int[numBins];
        BinTotals = new double[numBins];

        BinWidth = (BinMax - BinMin) / numBins;
    }

    public void Clear()
    {
        Array.Clear(Bins, 0, Bins.Length);
        Array.Clear(BinTotals, 0, BinTotals.Length);
    }

    public bool AddSample(float value)
    {
        int binTarget = (int)((value - BinMin) / BinWidth);

        if (binTarget < 0 || binTarget >= Bins.Length)
        {
            return false;
        }

        Bins[binTarget]++;
        BinTotals[binTarget] += value;

        return true;
    }

    public float GetBinAverage(int bin)
    {
        if ((bin < 0) || (bin > Bins.Length - 1))
        {
            throw new ArgumentOutOfRangeException();
        }

        if (Bins[bin] == 0)
        {
            return 0.0f;
        }

        return (float)(BinTotals[bin] / Bins[bin]);
    }

    public float GetBinCenter(int bin)
    {
        if ((bin < 0) || (bin > Bins.Length - 1))
        {
            throw new ArgumentOutOfRangeException();
        }

        return BinMin + (bin + 0.5f) * BinWidth;
    }

    public float GetBinTop(int bin)
    {
        if ((bin < 0) || (bin > Bins.Length - 1))
        {
            throw new ArgumentOutOfRangeException();
        }

        return BinMin + (bin + 1.0f) * BinWidth;
    }

    public float GetBinBottom(int bin)
    {
        if ((bin < 0) || (bin > Bins.Length - 1))
        {
            throw new ArgumentOutOfRangeException();
        }

        return BinMin + bin * BinWidth;
    }

    public int GetFirstBinWithMinCount(int minCount)
    {
        for (int i = 0; i < Bins.Length; ++i)
        {
            if (Bins[i] > minCount)
            {
                return i;
            }
        }

        return -1;
    }

    public int GetLastBinWithMinCount(int minCount)
    {
        for (int i = Bins.Length - 1; i >= 0; --i)
        {
            if (Bins[i] > minCount)
            {
                return i;
            }
        }

        return -1;
    }

    public int GetHighestBin()
    {
        int maxValue = 0;
        int maxBin = 0;

        for (int i = 0; i < Bins.Length; ++i)
        {
            if (Bins[i] > maxValue)
            {
                maxValue = Bins[i];
                maxBin = i;
            }
        }

        return maxBin;
    }
}
