Imports System.Text
Imports System.Security.Cryptography
Imports System.Configuration.ConfigurationSettings


'Namespace SpoofProofLogin


Public Class RandomStringGenerator
    Public Shared Function GenerateRandomString(ByVal iLength As Integer) As String

        'Dim iZero, iNine, iA, iZ, iCount, iRandNum As Integer
        Dim iStartBC, iEndBC, iStartSC, iEndSC, iCount, iTmpC As Integer
        Dim sRandomString As String = ""

        ' we'll need random characters, so a Random object 
        ' should probably be created...
        Dim rRandom As New Random(System.DateTime.Now.Millisecond)

        ' convert characters into their integer equivalents (their ASCII values)
        'iZero = Asc("0")
        'iNine = Asc("9")
        'iA = Asc("A")
        'iZ = Asc("Z")
        iStartSC = Asc("a")
        iEndSC = Asc("z")
        iStartBC = Asc("A")
        iEndBC = Asc("Z")

        ' now we loop as many times as is necessary to build the string 
        ' length we want
        While (iCount < iLength)
            ' we fetch a random number between our high and low values
            iTmpC = rRandom.Next(iStartBC, iEndSC)

            ' here's the cool part: we inspect the value of the random number, 
            ' and if it matches one of the legal values that we've decided upon,  
            ' we convert the number to a character and add it to our string
            If (((iTmpC >= iStartSC) And (iTmpC <= iEndSC) _
            Or (iTmpC >= iStartBC) And (iTmpC <= iEndBC))) Then
                sRandomString = sRandomString + Chr(iTmpC)
                iCount = iCount + 1
            End If

        End While
        ' finally, our random character string should be built, so we return it
        Return sRandomString

    End Function

    Public Shared Function HashMACMe(ByVal s As String) As String
        Dim b As Byte
        Dim HashValue() As Byte
        Dim retString As String = ""

        'Create a new instance of the UnicodeEncoding class to 
        'convert the string into an array of Unicode bytes.
        Dim UE As New UnicodeEncoding

        'Convert the string into an array of bytes.
        Dim MessageBytes As Byte() = UE.GetBytes(s & System.Configuration.ConfigurationManager.AppSettings("MACKey"))

        'Create a new instance of the SHA1Managed class to create 
        'the hash value.
        Dim SHhash As New SHA1Managed

        'Create the hash value from the array of bytes.
        HashValue = SHhash.ComputeHash(MessageBytes)

        'Return a hexadecimal representation of the String
        For Each b In HashValue
            retString += b.ToString("X2")
        Next

        Return retString
    End Function
End Class

'End Namespace
