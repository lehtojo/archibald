from google.cloud import texttospeech
import google.generativeai as genai
import sys
import os

# Read prompts from files
babble_prompt = open('options/babble/prompt.txt').read()
throw_left_prompt = open('options/throw_left/prompt.txt').read()
throw_right_prompt = open('options/throw_right/prompt.txt').read()
throw_opposite_side_prompt = open('options/throw_opposite_side/prompt.txt').read()

mock = True
mocks = {
    babble_prompt: open('options/babble/mock.txt').read(),
    throw_left_prompt: open('options/throw_left/mock.txt').read(),
    throw_right_prompt: open('options/throw_right/mock.txt').read(),
    throw_opposite_side_prompt: open('options/throw_opposite_side/mock.txt').read()
}

# Initialization:

# Make sure that we have the API key set
if "API_KEY" not in os.environ:
    raise ValueError("Please set the API key (API_KEY environment variable)")

genai.configure(api_key=os.environ["API_KEY"])
model = genai.GenerativeModel('gemini-1.5-flash')

def generate_text(prompt):
    if mock:
        return mocks[prompt].strip()
    else:
        return model.generate_content(prompt)

# Select the prompt based on the command line argument
if len(sys.argv) < 2:
    raise ValueError("Please specify the prompt option (babble, throw_left, throw_right, throw_opposite_side)")

prompt_option = sys.argv[1]
prompt = ""

if prompt_option == "babble":
    prompt = babble_prompt
elif prompt_option == "throw_left":
    prompt = throw_left_prompt
elif prompt_option == "throw_right":
    prompt = throw_right_prompt
elif prompt_option == "throw_opposite_side":
    prompt = throw_opposite_side_prompt
else:
    raise ValueError("Invalid prompt option")

# Generate the text
text = generate_text(prompt)

# Limit the number of characters to 4096
text = text[:4096]

if mock:
    # Load the mock audio file
    path = "options/" + prompt_option + "/mock.mp3"

    with open(path, "rb") as input:
        with open("output.mp3", "wb") as output:
            output.write(input.read())

    print(text)
    sys.exit(0)

# Synthesize speech from the generated text
client = texttospeech.TextToSpeechClient()

input_text = texttospeech.SynthesisInput(text=text)

voice = texttospeech.VoiceSelectionParams(
    language_code="es-US",
    # name="en-GB-News-M"
    name="es-US-Studio-B"
)

audio_config = texttospeech.AudioConfig(
    audio_encoding=texttospeech.AudioEncoding.LINEAR16,
    speaking_rate=0.5
)

response = client.synthesize_speech(
    request={ "input": input_text, "voice": voice, "audio_config": audio_config }
)

# Write response's audio content to a file
with open("output.mp3", "wb") as output:
    output.write(response.audio_content)

print(text)