using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace FALKR_SubstitutionCipher.Controllers
{
    public class DecryptController : Controller
    {

        private int _a;
        private int _b;

        private char[] _charactersByFrequency;
        private readonly char[] _alphabet = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', ' ' };

        private string _ciphertext;

        private int[] _cipherCharacterCountArray;

        private char[] _k;
        private char[] _k1;

        private string _dk;

        private int[,] _mD;
        private int[,] _mD1;
        private int[,] _mE;
        private int[,,] _mT;
        private int[,,] _mT1;
        private int[,,] _mU;

        private int _v;
        private int _v1;

        private char[] _s;

        private bool _terminate;
        private bool _step4;

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Decrypt(DecryptModel model)
        {
           
            if (!ModelState.IsValid)
                return View("Index");
            
            _charactersByFrequency = new[] { ' ', 'E', 'T', 'A', 'O', 'I', 'N', 'S', 'H', 'R', 'D', 'L', 'C', 'U', 'M', 'W', 'F', 'G', 'Y', 'P', 'B', 'V', 'K', 'J', 'X', 'Q', 'Z' };
           
            _terminate = false;
            _step4 = true;

            _ciphertext = model.Ciphertext.ToUpper();

            var originalLength = _ciphertext.Length;

            while (_ciphertext.Length < 1000)
                _ciphertext += " " + _ciphertext;

            _cipherCharacterCountArray = GenerateCipherCharacterCountArray();

            _s = CipherCharactersByFrequency();

            _mE = LoadDigram();

            //Step 0
            _a = 1;
            _b = 1;

            //Step 1
            _k = GenerateInitialKey();

            //Step 2
            _dk = DecryptWithKey(_k);

            _mD = GenerateDigram(_dk);
                                                

            //Step 3				
            _v = EvaluationFunction(_mD, _mE);

            while (!_terminate)
            {

                if (_step4)
                {

                    _step4 = false;

                    //Step 4
                    _k1 = new char[_k.GetLength(0)];
                    for (var i = 0; i < _k.GetLength(0); i++)
                        _k1[i] = _k[i];

                    //Step 5
                    _mD1 = new int[_mD.GetLength(0), _mD.GetLength(0)];
                    for (var i = 0; i < _mD.GetLength(0); i++)
                        for (var j = 0; j < _mD.GetLength(0); j++)
                            _mD1[i, j] = _mD[i, j];
                }

                //Step 6a
                var alphaBeta = GetAlphaBeta();
                var alpha = alphaBeta[0];
                var beta = alphaBeta[1];

                Swap(_k1, alpha, beta);

                //Step 6b
                _a++;

                //Step 6c
                if (_a + _b <= 27)
                {
                    //ugly but accurate to algorithm
                }
                else
                {

                    //Step 6d
                    _a = 1;

                    //Step 6e
                    _b++;

                    //Step 6f
                    if (_b == 27)
                    {
                        _terminate = true;
                        continue;
                    }
                }

                //Step 7
                SwapRows(alpha, beta, _mD1);
                SwapColumns(alpha, beta, _mD1);

                //Step 8
                _v1 = EvaluationFunction(_mD1, _mE);

                //Step 9
                if (_v1 >= _v)
                {
                    _step4 = true;
                    continue;
                }

                //Step 9b
                _a = 1;
                _b = 1;

                //Step 10
                _v = _v1;
                System.Diagnostics.Debug.WriteLine("new v = " + _v);

                //Step 11

                for (var i = 0; i < _k1.GetLength(0); i++)
                    _k[i] = _k1[i];

                //Step 12
                for (var i = 0; i < _mD1.GetLength(0); i++)
                    for (var j = 0; j < _mD1.GetLength(0); j++)
                        _mD[i, j] = _mD1[i, j];

                //Step 13

            }

            //-----------------------------
            // BEGIN TRIGRAM ANALYSIS
            //-----------------------------

            //Step 0
            _a = 1;
            _b = 1;

            //Step 1
            // initial key is key found with digram analysis.

            //Step 2
            _dk = DecryptWithKey(_k);

            _mT = GenerateTrigram(_dk);

            //Step 3				
            _mU = LoadTrigram();
            _v = EvaluationFunction(_mT, _mU);

            var terminateTrigram = false;
            _step4 = true;

            while (!terminateTrigram)
            {

                //Step 4
                if (_step4)
                {

                    _step4 = false;

                    _k1 = new char[_k.Length];
                    for (var i = 0; i < _k.Length; i++)
                        _k1[i] = _k[i];

                    // Step 5
                    // Create copy of Trigram
                    _mT1 = new int[_mT.GetLength(0),_mT.GetLength(0),_mT.GetLength(0)];
                    for (var i = 0; i < _mT1.GetLength(0); i++)
                        for (var j = 0; j < _mT1.GetLength(0); j++)
                            for (var k = 0; k < _mT1.GetLength(0); k++)
                                _mT1[i, j, k] = _mT[i, j, k];
                }

                //Step 6a
                var alphaBeta = GetAlphaBeta();
                var alpha = alphaBeta[0];
                var beta = alphaBeta[1];

                Swap(_k1, alpha, beta);
                //Step 6b
                _a++;
                //Step 6c
                if (_a + _b <= 27)
                {
                    //ugly but accurate to algorithm
                }
                else
                {
                    //Step 6d
                    _a = 1;
                    //Step 6e
                    _b++;
                    //Step 6f
                    if (_b == 27)
                        terminateTrigram = true;
                }

                //Step 7
                SwapWidth(alpha, beta, _mT1);
                SwapHeight(alpha, beta, _mT1);
                SwapDepth(alpha, beta, _mT1);

                //Step 8
                _v1 = EvaluationFunction(_mT1, _mU);

                //Step 9
                if (_v1 >= _v)
                {
                    _step4 = true;
                    continue;
                }

                //Step 9b
                _a = 1;
                _b = 1;

                //Step 10
                _v = _v1;

                //Step 11
                for (var i = 0; i < _k1.Length; i++)
                    _k[i] = _k1[i];

                //Step 12
                for (var i = 0; i < _mT1.GetLength(0); i++)
                    for (var j = 0; j < _mT1.GetLength(0); j++)
                        for (var k = 0; k < _mT1.GetLength(0); k++)
                            _mT[i, j, k] = _mT1[i, j, k];

                //Step13
            }

            model.Key = string.Concat(_k);
            model.Plaintext = DecryptWithKey(_k).Substring(0, originalLength);

            return View("Index", model);

        }

        private int[] GenerateCipherCharacterCountArray()
        {

            var characterCountArray = new int[27];

            for (var i = 0; i < _ciphertext.Length; i++)
            {
                if (_ciphertext.ElementAt(i) == ' ')
                    characterCountArray[26]++;
                else
                {
                    if (_ciphertext.ElementAt(i) >= 65 && _ciphertext.ElementAt(i) <= 90)
                        characterCountArray[_ciphertext.ElementAt(i) - 65]++;
                }
            }

            return characterCountArray;
        }

        private char[] CipherCharactersByFrequency()
        {

            var temp = new int[_cipherCharacterCountArray.GetLength(0)];

            for (var i = 0; i < _cipherCharacterCountArray.GetLength(0); i++)
                temp[i] = _cipherCharacterCountArray[i];

            var result = new char[temp.GetLength(0)];

            var maxIndex = 0;

            for (var i = 0; i < temp.GetLength(0); i++)
            {
                for (var j = 0; j < temp.GetLength(0); j++)
                    if (temp[j] > temp[maxIndex])
                        maxIndex = j;

                temp[maxIndex] = -1;

                if (maxIndex == 26)
                    result[i] = ' ';
                else
                    result[i] = (char)(maxIndex + 65);

            }

            return result;


        }

        public int[,] LoadDigram()
        {

            var e = new int[27, 27];

            var file = System.IO.File.ReadAllText("Digram.txt");

            var delimiters = new[] { '\n', '\t', '\r' };

            var words = file.Split(delimiters);

            var wordCount = 0;

            for (var i = 0; i < e.GetLength(0); i++)
                for (var j = 0; j < e.GetLength(0); j++)
                {
                    e[i, j] = Convert.ToInt32(words[wordCount]);
                    wordCount++;
                }

            return e;

        }

        private char[] GenerateInitialKey()
        {

            var unorderedK = new char[_cipherCharacterCountArray.GetLength(0)];

            var maxIndex = 0;

            for (var i = 0; i < _cipherCharacterCountArray.GetLength(0); i++)
            {
                for (var j = 0; j < _cipherCharacterCountArray.GetLength(0); j++)
                    if (_cipherCharacterCountArray[j] > _cipherCharacterCountArray[maxIndex])
                        maxIndex = j;

                _cipherCharacterCountArray[maxIndex] = -1;

                if (maxIndex == 26)
                    unorderedK[i] = ' ';
                else
                    unorderedK[i] = (char)(maxIndex + 65);

            }

            var k = new char[_cipherCharacterCountArray.GetLength(0)];

            for (var i = 0; i < k.GetLength(0); i++)
                if (_charactersByFrequency[i] == ' ')
                    k[26] = unorderedK[i];
                else
                    k[_charactersByFrequency[i] - 65] = unorderedK[i];

            return k;

        }

        private string DecryptWithKey(char[] k)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < _ciphertext.Length; i++)
                if ((_ciphertext.ElementAt(i) >= 65 && _ciphertext.ElementAt(i) <= 90) ||
                    _ciphertext.ElementAt(i) == ' ')
                    sb.Append(_alphabet[IndexOfCharacterInKey(_ciphertext.ElementAt(i), k)]);
                else
                    sb.Append(_ciphertext.ElementAt(i));

            var newText = sb.ToString(); // d_k(c)

            return newText.ToUpper();
        }

        private static int IndexOfCharacterInKey(char c, char[] k)
        {
            for (var i = 0; i < k.GetLength(0); i++)
                if (k[i] == c)
                    return i;

            return -1;

        }

        private int[,] GenerateDigram(string dk)
        {

            var digramD = new int[_alphabet.GetLength(0), _alphabet.GetLength(0)];

            for (var i = 0; i < dk.Length - 1; i++)
            {

                var c1 = dk.ElementAt(i);
                var c2 = dk.ElementAt(i + 1);

                var i1 = -1;
                var i2 = -1;

                if (c1 == ' ')
                    i1 = 26;
                else if (c1 >= 65 && c1 <= 90)
                    i1 = c1 - 65;

                if (c2 == ' ')
                    i2 = 26;
                else if (c2 >= 65 && c2 <= 90)
                    i2 = c2 - 65;

                if (i1 != -1 && i2 != -1)
                    digramD[i1, i2]++;
            }

            return digramD;

        }

        private static int EvaluationFunction(int[,] d, int[,] e)
        {

            var sum = 0;

            for (var i = 0; i < d.GetLength(0); i++)
                for (var j = 0; j < d.GetLength(0); j++)
                    sum += Math.Abs(d[i, j] - e[i, j]);

            return sum;

        }

        private char[] GetAlphaBeta()
        {

            var alpha = _s[_a - 1];
            var beta = _s[(_a + _b) - 1];

            char[] results = { alpha, beta };

            return results;

        }

        private void Swap(char[] k, char alpha, char beta)
        {

            var indexAlpha = 0;
            var indexBeta = 0;

            for (var i = 0; i < k.GetLength(0); i++)
            {
                if (_alphabet[i] != alpha) continue;
                indexAlpha = i;
                break;
            }

            for (var i = 0; i < k.GetLength(0); i++)
            {
                if (_alphabet[i] != beta) continue;
                indexBeta = i;
                break;
            }

            var temp = k[indexAlpha];
            k[indexAlpha] = k[indexBeta];
            k[indexBeta] = temp;

        }

        private void SwapRows(char alpha, char beta, int[,] d)
        {

            var indexAlpha = 0;
            var indexBeta = 0;

            for (var i = 0; i < _alphabet.GetLength(0); i++)
            {
                if (_alphabet[i] != alpha) continue;
                indexAlpha = i;
                break;
            }

            for (var i = 0; i < _alphabet.GetLength(0); i++)
            {
                if (_alphabet[i] != beta) continue;
                indexBeta = i;
                break;
            }

            for (var i = 0; i < d.GetLength(0); i++)
            {
                var temp = d[indexAlpha,i];
                d[indexAlpha,i] = d[indexBeta,i];
                d[indexBeta,i] = temp;
            }
        }

        private void SwapColumns(char alpha, char beta, int[,] d)
        {

            var indexAlpha = 0;
            var indexBeta = 0;

            for (var i = 0; i < _alphabet.GetLength(0); i++)
            {
                if (_alphabet[i] != alpha) continue;
                indexAlpha = i;
                break;
            }

            for (var i = 0; i < _alphabet.GetLength(0); i++)
            {
                if (_alphabet[i] != beta) continue;
                indexBeta = i;
                break;
            }

            for (var i = 0; i < d.GetLength(0); i++)
            {
                var temp = d[i,indexAlpha];
                d[i,indexAlpha] = d[i,indexBeta];
                d[i,indexBeta] = temp;
            }
        }

        private int[,,] LoadTrigram()
        {

            var e = new int[27,27,27];

            var file = System.IO.File.ReadAllText("Trigram.txt");

            var delimiters = new[] { '\n', '\t', '\r' };

            var words = file.Split(delimiters);

            var wordCount = 0;

                while(wordCount < words.GetLength(0))
                {
                    var trigram = words[wordCount];
                    var symbol1 = trigram.ElementAt(0);
                    var symbol2 = trigram.ElementAt(1);
                    var symbol3 = trigram.ElementAt(2);
                    int index1;
                    if (symbol1 == '#')
                        index1 = 26;
                    else
                        index1 = symbol1 - 65;

                    int index2;
                    if (symbol2 == '#')
                        index2 = 26;
                    else
                        index2 = symbol2 - 65;

                    int index3;
                    if (symbol3 == '#')
                        index3 = 26;
                    else
                        index3 = symbol3 - 65;

                    var frequency = Convert.ToInt32(words[wordCount+1]);
                    e[index1,index2,index3] = frequency;

                wordCount = wordCount + 2;
            }

            return e;
        }

        private static int EvaluationFunction(int[,,] d, int[,,] e)
        {

            var sum = 0;

            for (var i = 0; i < 27; i++)
                for (var j = 0; j < 27; j++)
                    for (var k = 0; k < 27; k++)
                        sum += Math.Abs(d[i, j, k] - e[i, j, k]);

            return sum;
        }

        private static int[,,] GenerateTrigram(string dk)
        {

            var trigramD = new int[27,27,27];

            for (var i = 0; i < trigramD.GetLength(0); i++)
                for (var j = 0; j < trigramD.GetLength(0); j++)
                    for (var k = 0; k < trigramD.GetLength(0); k++)
                        trigramD[i, j, k] = 0;

            for (var i = 0; i < dk.Length - 2; i++)
            {

                var c1 = dk.ElementAt(i);
                var c2 = dk.ElementAt(i + 1);
                var c3 = dk.ElementAt(i + 2);

                var i1 = -1;
                var i2 = -1;
                var i3 = -1;

                if (c1 == ' ')
                    i1 = 26;
                else if (c1 >= 65 && c1 <= 90)
                    i1 = c1 - 65;

                if (c2 == ' ')
                    i2 = 26;
                else if (c2 >= 65 && c2 <= 90)
                    i2 = c2 - 65;

                if (c3 == ' ')
                    i3 = 26;
                else if (c3 >= 65 && c3 <= 90)
                    i3 = c3 - 65;

                if (i1 != -1 && i2 != -1 && i3 != -1)
                    trigramD[i1, i2, i3]++;
            }

            return trigramD;
        }

        protected IActionResult SwapCharacters(DecryptModel model)
        {

            _ciphertext = model.Ciphertext;

            var s = model.Key;

            var key = new char[27];

            for (var i = 0; i < key.Length; i++)
                key[i] = model.Ciphertext.ElementAt(i);

            var indexChar1 = -1;
            var indexChar2 = -1;

            for (var i = 0; i < _alphabet.Length; i++)
                if (model.From.ElementAt(0) == _alphabet[i])
                    indexChar1 = i;

            for (var i = 0; i < _alphabet.Length; i++)
                if (model.To.ElementAt(0) == _alphabet[i])
                    indexChar2 = i;

            var temp = key[indexChar1];
            key[indexChar1] = key[indexChar2];
            key[indexChar2] = temp;

            model.Ciphertext = string.Concat(key);
            model.Plaintext = DecryptWithKey(key);

            return View("Index", model);

        }

        private void SwapWidth(char alpha, char beta, int[,,] t)
        {
            var indexAlpha = IndexOfCharacterInKey(alpha, _alphabet);
            var indexBeta = IndexOfCharacterInKey(beta, _alphabet);

            for (var i = 0; i < t.GetLength(0); i++)
                for (var j = 0; j < t.GetLength(0); j++)
                {
                    var temp = t[indexAlpha, i, j];
                    t[indexAlpha, i, j] = t[indexBeta, i, j];
                    t[indexBeta, i, j] = temp;
                }
        }

        private void SwapHeight(char alpha, char beta, int[,,] t)
        {
            var indexAlpha = IndexOfCharacterInKey(alpha, _alphabet);
            var indexBeta = IndexOfCharacterInKey(beta, _alphabet);

            for (var i = 0; i < t.GetLength(0); i++)
                for (var j = 0; j < t.GetLength(0); j++)
                {
                    var temp = t[i, indexAlpha, j];
                    t[i, indexAlpha, j] = t[i, indexBeta, j];
                    t[i, indexBeta, j] = temp;
                }
        }

        private void SwapDepth(char alpha, char beta, int[,,] t)
        {
            var indexAlpha = IndexOfCharacterInKey(alpha, _alphabet);
            var indexBeta = IndexOfCharacterInKey(beta, _alphabet);

            for (var i = 0; i < t.GetLength(0); i++)
                for (var j = 0; j < t.GetLength(0); j++)
                {
                    var temp = t[i, j, indexAlpha];
                    t[i, j, indexAlpha] = t[i, j, indexBeta];
                    t[i, j, indexBeta] = temp;
                }
        }

    }
}
