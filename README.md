# Visual Pinball Engine - MPF Gamelogic Engine
*Enables the Mission Pinball Framework to drive VPE*

## Setup

You currently need Python and MPF installed locally.

1. Install Python 3
2. `pip install --pre mpf mpf-mc`

Or, if you already have it:

`pip install mpf mpf-mc --pre --upgrade`

After that, `mpf --version` should return at least **MPF v0.55.0-dev.12**.

## Usage

Test code in the repo. Init:

```cs
var mpfApi = new MpfApi(@"path\to\machine\folder");

// spawn MPF and connect to it
await mpfApi.Launch();

// start MPF
await mpfApi.Start();

// retrieve machine configuration
var descr = await mpfApi.GetMachineDescription();

Console.WriteLine($"Description: {descr}");
```

## License

[MIT](LICENSE)