from fastapi import FastAPI
from pydantic import BaseModel
from transformers import AutoTokenizer, AutoModel
import torch

app = FastAPI()

# Load CodeBERT model
tokenizer = AutoTokenizer.from_pretrained("microsoft/codebert-base")
model = AutoModel.from_pretrained("microsoft/codebert-base")

class CodeRequest(BaseModel):
    code: str
    language: str

@app.post("/analyze")
def analyze(request: CodeRequest):

    inputs = tokenizer(
        request.code,
        return_tensors="pt",
        truncation=True,
        padding=True
    )

    outputs = model(**inputs)

    # Basic embedding logic
    embedding = outputs.last_hidden_state.mean().item()

    return {
        "score": 85,
        "complexity": "O(n)",
        "pattern": "Detected",
        "feedback": "Code analyzed using CodeBERT"
    }